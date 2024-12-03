using Microsoft.Extensions.Localization;
using Warehouse.Domain.Data.Identity;
using Warehouse.Domain.Data.Repositories;
using Warehouse.Domain.Models.Common;
using Warehouse.Domain.Models.Entities;
using Warehouse.Web.Components.Account;
using Warehouse.Web.Localization;
using Warehouse.Web.ViewModels.Company;

namespace Warehouse.Web.Services;

public class CompanyService : BaseService<CompanyService>
{
    private readonly CompanyRepository _companyRepository;
    private readonly UserService _userService;

    public CompanyService(ILogger<CompanyService> logger, IStringLocalizer<LabelResources> stringLocalizer, CompanyRepository companyRepository, UserService userService) : base(logger, stringLocalizer)
    {
        _companyRepository = companyRepository;
        _userService = userService;
    }

    public async Task<Result> VerifyTaxIdentificationNumber(string taxIdentificationNumber)
    {
        var taxNumberInUse =
            await _companyRepository.CheckIfTaxIdentificationNumberIsInUse(taxIdentificationNumber);

        return taxNumberInUse
            ? Result.Failure("Given tax identification number is already in use")
            : Result.Success();
    }

    public async Task<Result<Guid?>> RegisterCompany(RegisterCompanyModel registerCompanyModel)
    {
        var numberVerification = await VerifyTaxIdentificationNumber(registerCompanyModel.TaxIdentificationNumber);

        if (!numberVerification.Succeeded)
        {
            return Result<Guid?>.Failure(numberVerification.Errors);
        }

        var company = new Company()
        {
            Name = registerCompanyModel.Name,
            TaxIdentificationNumber = registerCompanyModel.TaxIdentificationNumber,
            ManagerId = registerCompanyModel.UserId,
            Address = new Address
            {
                AddressLine1 = registerCompanyModel.AddressLine1,
                AddressLine2 = registerCompanyModel.AddressLine2,
                City = registerCompanyModel.City,
                State = registerCompanyModel.State,
                ZipCode = registerCompanyModel.ZipCode,
                Country = registerCompanyModel.Country
            },
        };

        company.FillCreated(registerCompanyModel.UserId);

        _companyRepository.Context.Add(company);
        await _companyRepository.Context.SaveChangesAsync();

        await _userService.AddExistingUserToCompany(registerCompanyModel.UserId, company.Id);

        return Result<Guid?>.Success(company.Id);
    }

    public async Task<Company?> FindCompany(Guid companyId)
    {
        var company = await _companyRepository.GetById(companyId);
        return company;
    }

    /// <summary>
    /// Retrieves the company name of the currently logged-in user or by using providedId.
    /// </summary>
    public async Task<string> GetCurrentCompanyName(Guid? companyId = null)
    {
        if (companyId is null)
        {
            var user = await _userService.GetCurrentUser();
            if (user is null || user.CompanyId == null)
            {
                return string.Empty;
            }

            companyId = user.CompanyId;
        }

        var company = await FindCompany(companyId.Value);
        return await Task.FromResult($"{company?.Name}");
    }
}