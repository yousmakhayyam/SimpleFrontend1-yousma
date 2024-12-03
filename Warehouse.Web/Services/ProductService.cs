using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Warehouse.Domain.Data.Repositories;
using Warehouse.Domain.Models.Common;
using Warehouse.Domain.Models.Entities;
using Warehouse.Web.Helpers;
using Warehouse.Web.Infrastructure;
using Warehouse.Web.Localization;
using Warehouse.Web.ViewModels;
using Warehouse.Web.ViewModels.Products;

namespace Warehouse.Web.Services;

public class ProductService : BaseService<ProductService>
{
    private readonly ObjectStoreHelper _objectStoreHelper;
    private readonly ProductRepository _productRepository;

    public ProductService(ILogger<ProductService> logger,
        IStringLocalizer<LabelResources> stringLocalizer,
        ObjectStoreHelper objectStoreHelper,
        ProductRepository productRepository) : base(logger,
        stringLocalizer)
    {
        _objectStoreHelper = objectStoreHelper;
        _productRepository = productRepository;
    }

    public async Task<ProductImageModel> SaveImageAsync(IBrowserFile file, Stream fileStream)
    {
        var productImage = new ProductImageModel
        {
            FileId = Guid.NewGuid(),
            FileName = file.Name,
            IsDefault = false
        };

        await _objectStoreHelper.Create(fileStream, productImage.GetFullFileName);

        return productImage;
    }

        

    public async Task<Result> CreateOrEditProduct(ProductModel product, string userId)
    {
        var existingProduct = await _productRepository.GetById(product.Id);
        var isNew = false;

        if (existingProduct is null)
        {
            existingProduct = new Product
            {
                Id = Guid.NewGuid(),
                OwnerId = userId,
                CreatedByCompanyId = product.CompanyId,
            };

            isNew = true;
        }

        existingProduct.Name = product.Name;
        existingProduct.Description = product.Description;
        existingProduct.Price = product.Price;
        existingProduct.Category = product.Category;
        existingProduct.StockQuantity = product.Quantity;

        if (isNew)
        {
            existingProduct.FillCreated(userId);
            _productRepository.Context.Add(existingProduct);
        }
        else
        {
            existingProduct.FillModified(userId);
            _productRepository.Context.Update(existingProduct);
        }

        // Recreate product images
        foreach (var productImage in existingProduct.Images)
        {
            _productRepository.Context.Remove(productImage);
        }

        foreach (var productImage in product.Images)
        {
            var image = new ProductImage
            {
                Id = Guid.NewGuid(),
                FileId = productImage.FileId,
                FileName = productImage.FileName,
                IsDefault = productImage.FileId == product.DefaultImageFileId,
                ProductId = existingProduct.Id,
            };

            _productRepository.Context.Add(image);
        }

        await _productRepository.Context.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<(List<Product> Results, int TotalCount)> FindAll(string searchTerm,
        int currentPage,
        int pageSize,
        SortingDirection? sortingDirection = null,
        string? sortingProperty = null,
        string? userId = null,
        Guid? companyId = null)
    {
        // Validate input parameters
        if (currentPage < 1) currentPage = 1;
        if (pageSize < 1) pageSize = 10;

        // Define the query
        IQueryable<Product> query = _productRepository.Context.Products.Include(p => p.Images);

        // Apply filter for getting only products for given user
        if (!string.IsNullOrEmpty(userId))
        {
            query = query.Where(p => p.OwnerId == userId);
        }

        // Apply filter for getting only products for given company
        if (companyId.HasValue)
        {
            query = query.Where(p => p.CreatedByCompanyId == companyId);
        }

        // Apply search term filter if provided
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.ToLower();
            query = query.Where(p =>
                p.Name.ToLower().Contains(searchTerm) || p.Category.ToLower().Contains(searchTerm));
        }

        // Get the total count before applying pagination
        var totalCount = await query.CountAsync();

        if (!string.IsNullOrEmpty(sortingProperty))
        {
            query = sortingProperty switch
            {
                "Price" when sortingDirection == SortingDirection.Asc => query.OrderBy(p => p.Price.Amount),
                "Price" when sortingDirection == SortingDirection.Desc => query.OrderByDescending(p => p.Price.Amount),
                "Name" when sortingDirection == SortingDirection.Asc => query.OrderBy(p => p.Name),
                "Name" when sortingDirection == SortingDirection.Desc => query.OrderByDescending(p => p.Name),
                "Description" when sortingDirection == SortingDirection.Asc => query.OrderBy(p => p.Description),
                "Description" when sortingDirection == SortingDirection.Desc => query.OrderByDescending(p => p.Description),
                _ => query
            };
        }

        // Apply pagination
        var results = await query
            .Skip((currentPage - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // Return the results and the total count
        return (results, totalCount);
    }

    public async Task<Product?> GetProductById(Guid productId)
    {
        var result = await _productRepository.GetById(productId);

        return result;
    }

    public async Task<List<Product>> GetNewestProducts(int size, string? ownerId = null, Guid? companyId = null)
    {
        IQueryable<Product> query = _productRepository.Context.Products.Include(p => p.Images);

        if (!string.IsNullOrWhiteSpace(ownerId))
        {
            query = query.Where(r => r.OwnerId == ownerId);
        }

        if (companyId.HasValue)
        {
            query = query.Where(r => r.CreatedByCompanyId == companyId);
        }

        var results = await query.OrderByDescending(p => p.Created).Take(size).ToListAsync();

        return results;
    }
}