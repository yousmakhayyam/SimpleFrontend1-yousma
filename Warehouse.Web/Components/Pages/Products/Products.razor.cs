using Microsoft.AspNetCore.Components;
using Warehouse.Domain.Models.Entities;
using Warehouse.Web.Services;
using Warehouse.Web.ViewModels;
using Warehouse.Web.ViewModels.Products;

namespace Warehouse.Web.Components.Pages.Products;

public partial class Products
{
    [Inject] private ProductService ProductService { get; set; } = null!;
    [Inject] private UserService UserService{ get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;

    private string ViewMode { get; set; } = "all";

    private List<ProductModel> DisplayedProducts = new List<ProductModel>();
    private string SearchTerm = string.Empty;
    private int CurrentPage = 1;
    private int PageSize = 25;
    private int TotalCount = 0;

    private string? SortProperty { get; set; } = "name";
    private SortingDirection? SortDirection { get; set; } = null; // "asc", "desc"

    protected override async Task OnInitializedAsync()
    {
        var queryString = NavigationManager.ToAbsoluteUri(NavigationManager.Uri).Query;
        if (queryString.Contains("viewMode=user"))
        {
            ViewMode = "user";
        } else if (queryString.Contains("viewMode=company"))
        {
            ViewMode = "company";
        }
        else
        {
            ViewMode = "all";
        }
        
        await LoadProducts();
    }

    private int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    private async Task LoadProducts()
    {
        List<Product> results;
        int totalCount;

        var currentUser = await UserService.GetCurrentUser();
        switch (ViewMode)
        {
            case "user":
                (results, totalCount) = await ProductService.FindAll(SearchTerm, CurrentPage, PageSize, SortDirection, SortProperty, currentUser?.Id, null);
                break;
            case "company":
                (results, totalCount) = await ProductService.FindAll(SearchTerm, CurrentPage, PageSize, SortDirection, SortProperty, null, currentUser?.CompanyId);
                break;
            default:
                (results, totalCount) = await ProductService.FindAll(SearchTerm, CurrentPage, PageSize, SortDirection, SortProperty, null, null);
                break;
        }

        DisplayedProducts = results.Select(r => new ProductModel
        {
            Id = r.Id,
            Name = r.Name,
            Description = r.Description,
            Category = r.Category,
            Price = r.Price,
            Images = r.Images.Select(i => new ProductImageModel
            {
                FileId = i.FileId,
                FileName = i.FileName,
                IsDefault = i.IsDefault
            }).ToList()
        }).ToList();
        TotalCount = totalCount;
    }

    private async Task ChangePage(int pageNumber)
    {
        if (pageNumber < 1 || pageNumber > TotalPages)
            return;

        CurrentPage = pageNumber;
        await LoadProducts();
    }

    private async Task OnSearchInput(ChangeEventArgs e)
    {
        SearchTerm = e.Value?.ToString() ?? string.Empty;
        CurrentPage = 1; // Reset to the first page on search
        await LoadProducts();
    }

    private void OpenProductDetails(Guid productId)
    {
        Console.WriteLine($"Navigating to details for Product ID: {productId}");
        NavigationManager.NavigateTo($"/products/details/{productId}");   
    }

    private async Task OnSortChanged(string property)
    {
        // Cycle through sorting states: none -> asc -> desc -> none
        if (SortProperty == property)
        {
            SortDirection = SortDirection switch
            {
                null => SortingDirection.Asc,
                SortingDirection.Asc => SortingDirection.Desc,
                SortingDirection.Desc => null,
                _ => null
            };
        }
        else
        {
            // Reset to ascending when switching to a new property
            SortProperty = property;
            SortDirection = SortingDirection.Asc;
        }

        CurrentPage = 1; // Reset to the first page when sorting
        await LoadProducts();
    }

    private string GetSortIcon(SortingDirection? direction)
    {
        return direction switch
        {
            SortingDirection.Asc => "▲",
            SortingDirection.Desc => "▼",
            _ => ""
        };
    }
}