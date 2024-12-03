using Microsoft.AspNetCore.Identity;
using Warehouse.Domain.Data.Identity;
using Warehouse.Domain.Models.Entities;
using Warehouse.Domain.Models.Enums;
using Warehouse.Web.Services;
using Warehouse.Web.ViewModels.Products;

namespace Warehouse.Web.MappingProfiles;

public class MapProductsToModel
{
    private readonly CompanyService _companyService;
    private readonly UserManager<ApplicationUser> _userManager;

    public MapProductsToModel(CompanyService companyService, UserManager<ApplicationUser> userManager)
    {
        _companyService = companyService;
        _userManager = userManager;
    }

    public async Task<ProductModel> Map(Product entity)
    {
        var user = await _userManager.FindByIdAsync(entity.OwnerId);
        Company? company = new Company();
        if (entity.CreatedByCompanyId.HasValue)
        {
            company = await _companyService.FindCompany(entity.CreatedByCompanyId.GetValueOrDefault());
        }

        return new ProductModel
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            Category = entity.Category,
            Price = entity.Price,
            Images = entity.Images.Select(r => new ProductImageModel
            {
                FileId = r.FileId,
                FileName = r.FileName,
                IsDefault = r.IsDefault
            }).ToList(),
            Quantity = entity.StockQuantity,
            OwnerId = entity.OwnerId,
            OwnerName = user?.GetFullName ?? string.Empty,
            AddedBy = entity.CreatedByCompanyId.HasValue ? ProductAdditionTypes.Company : ProductAdditionTypes.Personal,
            CompanyId = entity.CreatedByCompanyId,
            CompanyName = company?.Name ?? string.Empty,
            DefaultImageFileId = entity.Images.FirstOrDefault(r => r.IsDefault)?.FileId,
        };
    }
}