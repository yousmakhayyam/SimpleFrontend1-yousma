using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Warehouse.Web.MappingProfiles;
using Warehouse.Web.Services;
using Warehouse.Web.ViewModels.Products;

namespace Warehouse.Web.Components.Pages;

public partial class Home
{
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;
    [Inject] private ProductService ProductService { get; set; } = null!;
    [Inject] private MapProductsToModel MapProductsToModel { get; set; } = null!;

    private string UserName { get; set; }
    private List<ProductModel> RecommendedProducts = new List<ProductModel>();

    protected override async Task OnInitializedAsync()
    {
        // Fetch user information
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        UserName = user.Identity?.Name ?? "User";

        // Load recommended products (fake data for now)
        RecommendedProducts = await LoadRecommendedProducts();
    }

    private async Task<List<ProductModel>> LoadRecommendedProducts()
    {
        var products = await ProductService.GetNewestProducts(3);
        var results = new List<ProductModel>();

        foreach (var product in products)
        {
            results.Add(await MapProductsToModel.Map(product));
        }

        return results;
    }

    private void NavigateToProfile()
    {
        NavigationManager.NavigateTo("/profile");
    }

    private void NavigateToProducts()
    {
        NavigationManager.NavigateTo("/products");
    }

    private void NavigateToLogin()
    {
        NavigationManager.NavigateTo("/Account/Login");
    }

    private async Task Logout()
    {
        // Perform logout logic
        await AuthenticationStateProvider.GetAuthenticationStateAsync();
        NavigationManager.NavigateTo("/Account/Logout", forceLoad: true);
    }

    private void GoToProductDetails(Guid productId)
    {
        NavigationManager.NavigateTo($"/products/details/{productId}");
    }

}