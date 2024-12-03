using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Warehouse.Web.MappingProfiles;
using Warehouse.Web.Services;
using Warehouse.Web.ViewModels.Products;

namespace Warehouse.Web.Components.Pages.Products;

[Authorize]
public partial class EditCreateProduct
{
    [Inject] protected NavigationManager NavigationManager { get; set; } = null!;
    [Inject] protected AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;
    [Inject] protected CompanyService CompanyService { get; set; } = null!;
    [Inject] protected ProductService ProductService { get; set; } = null!;
    [Inject] protected MapProductsToModel MapProductsToModel { get; set; } = null!;
    [Inject] protected UserService UserService { get; set; } = null!;

    [Parameter]
    public Guid? ProductId { get; set; }

    protected List<string> ErrorMessages { get; set; } = new();
    protected ProductModel Model { get; set; } = new ProductModel();
    protected bool UserHasCompanyAccess { get; set; }
    protected bool EditMode { get; set; } = false;

    protected override async void OnInitialized()
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        
        if (user.Identity?.IsAuthenticated ?? false)
        {
            var companyIdClaim = user.FindFirst("CompanyId")?.Value;
            UserHasCompanyAccess = companyIdClaim != null;

            if (ProductId.HasValue)
            {
                var product = await ProductService.GetProductById(ProductId.Value);
                if (product is not null)
                {
                    Model = await MapProductsToModel.Map(product);
                    EditMode = true;
                }
                else
                {
                    NavigationManager.NavigateTo("/products");
                }
            }
        }
        else
        {
            NavigationManager.NavigateTo("/products");
        }
    }

    protected async Task HandleImageUpload(InputFileChangeEventArgs e)
    {
        const long maxFileSize = 5 * 1024 * 1024; // 5 MB size limit

        foreach (var file in e.GetMultipleFiles())
        {
            if (file.Size > maxFileSize)
            {
                // You might want to show a message to the user or handle the error gracefully here.
                Console.WriteLine($"File {file.Name} exceeds the maximum allowed size of 5 MB.");
                continue;
            }

            // Adjust to open the file with a custom limit
            await using var stream = file.OpenReadStream(maxFileSize);

            // Save the file to the server (you could also do this in ProductService.SaveImageAsync)
            var productImage = await ProductService.SaveImageAsync(file, stream);

            if (!Model.Images.Any())
            {
                productImage.IsDefault = true;
                Model.DefaultImageFileId = productImage.FileId;
            }

            Model.Images.Add(productImage);
        }
    }

    protected void RemoveImage(ProductImageModel image)
    {
        Model.Images.Remove(image);

        // TODO: notify the server to delete the image
    }

    protected async Task SaveProduct()
    {
        var userId = UserService.GetCurrentUserId();

        if (string.IsNullOrEmpty(userId))
        {
            return;
        }

        ValidateModel();

        if (ErrorMessages.Any())
        {
            return;
        }

        var result = await ProductService.CreateOrEditProduct(Model, userId);

        if (result.Succeeded)
        {
            NavigationManager.NavigateTo("/products");
        } 
    }

    private void ValidateModel()
    {
        ErrorMessages.Clear();

        if (string.IsNullOrEmpty(Model.Category))
        {
            ErrorMessages.Add("Category must be selected for product.");
        }

        if (string.IsNullOrWhiteSpace(Model.Name))
        {
            ErrorMessages.Add("Product name is required.");
        }

        if (!Model.Images.Any())
        {
            ErrorMessages.Add("Product must contains at least one image");
        }
    }

    protected void Cancel()
    {
        NavigationManager.NavigateTo("/products");
    }
}
