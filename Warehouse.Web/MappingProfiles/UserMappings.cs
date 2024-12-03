using Microsoft.AspNetCore.Identity;
using Warehouse.Domain.Data.Identity;
using Warehouse.Web.Services;
using Warehouse.Web.ViewModels.User;

namespace Warehouse.Web.MappingProfiles;

public class UserMappings
{
    private readonly CompanyService _companyService;

    public UserMappings(CompanyService companyService)
    {
        _companyService = companyService;
    }

    public async Task<UserModel> ToUserModel(ApplicationUser user)
    {
        var companyName = await _companyService.GetCurrentCompanyName(user.CompanyId);
        return new UserModel
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user?.Email ?? string.Empty,
            Role = user.Role,
            CompanyId = user?.CompanyId,
            Company = companyName
        };
    }
}