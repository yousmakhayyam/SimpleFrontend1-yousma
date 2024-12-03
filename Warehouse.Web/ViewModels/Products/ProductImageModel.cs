namespace Warehouse.Web.ViewModels.Products;

public class ProductImageModel
{
    public Guid FileId { get; set; }
    public string FileName { get; set; }
    public bool IsDefault { get; set; }

    public string GetFullFileName => $"{FileId}_{FileName}";
}