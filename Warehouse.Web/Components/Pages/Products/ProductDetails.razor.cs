using Microsoft.AspNetCore.Components;
using Warehouse.Web.MappingProfiles;
using Warehouse.Web.Services;
using Warehouse.Web.ViewModels.Products;

namespace Warehouse.Web.Components.Pages.Products;

public partial class ProductDetails
{
    [Parameter]
    public Guid ProductId { get; set; }

    [Inject] private ProductService ProductService { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private MapProductsToModel MapProductsToModel { get; set; } = null!;
    [Inject] private UserService UserService { get; set; } = null!;

    private ProductModel Model { get; set; } = new();
    private string? DefaultImage { get; set; }
    private string? CurrentUserId { get; set; }

    protected override async Task OnInitializedAsync()
    {
        CurrentUserId = UserService.GetCurrentUserId();

        await LoadProductDetails();
    }

    private async Task LoadProductDetails()
    {
        var product = await ProductService.GetProductById(ProductId);
        if (product is null)
        {
            NavigationManager.NavigateTo("/error");
            return;
        }

        Model = await MapProductsToModel.Map(product);
        if (Model?.Images != null && Model.Images.Any())
        {
            DefaultImage = Model.Images.FirstOrDefault(img => img.IsDefault)?.FileName ?? Model.Images.First().FileName;
        }
    }
}