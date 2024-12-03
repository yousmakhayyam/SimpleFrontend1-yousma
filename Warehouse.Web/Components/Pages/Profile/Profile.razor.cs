using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Warehouse.Domain.Data.Identity;
using Warehouse.Web.MappingProfiles;
using Warehouse.Web.Services;
using Warehouse.Web.ViewModels.User;

namespace Warehouse.Web.Components.Pages.Profile;

[Authorize]
public partial class Profile
{
    [Inject] private UserService UserService { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;

    [Inject] private CompanyService CompanyService { get; set; } = null!;
    [Inject] private ProductService ProductService { get; set; } = null!;
    [Inject] private MapProductsToModel MapProductsToModel { get; set; } = null!;
    private ProfileModel Model { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        var user = await UserService.GetCurrentUser();
        if (user == null)
        {
            NavigationManager.NavigateTo("/");
            return;
        }

        await LoadUsers(user); 
        await LoadNewestProducts(user);
    }

    public async Task LoadUsers(ApplicationUser user)
    {
        Model.FirstName = user.FirstName;
        Model.LastName = user.LastName;
        Model.Email = user.Email ?? string.Empty;
        Model.JoinedOn = user.CreatedOn;

        if (user.CompanyId.HasValue)
        {
            var company = await CompanyService.FindCompany(user.CompanyId.Value);
            Model.Company = company?.Name;
        }
    }

    private async Task LoadNewestProducts(ApplicationUser user)
    {
        var userProducts = await ProductService.GetNewestProducts(5, user.Id);

        foreach (var product in userProducts)
        {
            Model.NewestProducts.Add(await MapProductsToModel.Map(product));
        }
    }
}