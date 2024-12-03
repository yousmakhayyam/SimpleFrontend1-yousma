using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Warehouse.Domain.Data.Identity;
using Warehouse.Web.MappingProfiles;
using Warehouse.Web.Services;
using Warehouse.Web.ViewModels.Company;
using Address = Warehouse.Web.ViewModels.Common.Address;

namespace Warehouse.Web.Components.Pages.Company;

[Authorize(Roles = "CompanyUser,CompanyManager")]
public partial class Dashboard
{
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private CompanyService CompanyService { get; set; } = null!;
    [Inject] private ProductService ProductService { get; set; } = null!;
    [Inject] private MapProductsToModel MapProductsToModel { get; set; } = null!;
    [Inject] private UserService UserService { get; set; } = null!;

    protected CompanyModel? Model { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var user = await UserService.GetCurrentUser();

        if (user is not { CompanyId: not null })
        {
            NavigationManager.NavigateTo("/");
            return;
        }

        await LoadCompany(user); 
        await LoadNewestProducts(user);
    }

    private async Task LoadCompany(ApplicationUser user)
    {
        // Fetch the company the user belongs to
        var company = await CompanyService.FindCompany(user.CompanyId.GetValueOrDefault());
        if (company is null)
        {
            NavigationManager.NavigateTo("/");
            return;
        }

        // Find the manager by comparing the ManagerId to the Users' Ids
        var companyManager = company.Users.FirstOrDefault(u => u.Id == company.ManagerId);

        // Populate the Company model for the UI
        Model = new CompanyModel
        {
            Name = company.Name,
            TaxId = company.TaxIdentificationNumber,
            ManagerName = companyManager?.GetFullName ?? "N/A",
            Address = new Address()
            {
                AddressLine1 = company.Address.AddressLine1,
                AddressLine2 = company.Address.AddressLine2,
                City = company.Address.City,
                Country = company.Address.Country,
                State = company.Address.State,
                ZipCode = company.Address.ZipCode
            }
        };
    }

    private async Task LoadNewestProducts(ApplicationUser user)
    {
        var userProducts = await ProductService.GetNewestProducts(5, companyId: user.CompanyId);

        foreach (var product in userProducts)
        {
            Model.NewestProducts.Add(await MapProductsToModel.Map(product));
        }
    }

    protected void ViewProductDetails(Guid productId)
    {
        NavigationManager.NavigateTo($"/products/details/{productId}");
    }
}