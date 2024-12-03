using System.ComponentModel.DataAnnotations;
using Warehouse.Domain.Models.Common;
using Warehouse.Domain.Models.Enums;
using Warehouse.Web.Components.Pages.Products;
using Money = Warehouse.Domain.Models.Common.Money;

namespace Warehouse.Web.ViewModels.Products;

public class ProductModel
{
    public Guid Id { get; set; }

    [Required]
    [StringLength(250)] 
    public string Name { get; set; }
    
    public string Description { get; set; }
    
    [Required] 
    public string Category { get; set; }
    public Money Price { get; set; } = new Money(0, 0, Currency.PLN);
    public List<ProductImageModel> Images { get; set; } = new List<ProductImageModel>();
    public ProductAdditionTypes AddedBy { get; set; }
    public int Quantity { get; set; }
    public string OwnerId { get; set; }
    public string OwnerName { get; set; }
    public Guid? CompanyId { get; set; }
    public string CompanyName { get; set; }

    public Guid? DefaultImageFileId { get; set; }
}