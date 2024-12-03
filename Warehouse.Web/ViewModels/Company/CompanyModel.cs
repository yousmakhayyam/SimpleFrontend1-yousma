using Warehouse.Web.ViewModels.Common;
using Warehouse.Web.ViewModels.Products;

namespace Warehouse.Web.ViewModels.Company;

public class CompanyModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string TaxId { get; set; }
    public string ManagerName { get; set; }
    public Address Address { get; set; }
    public List<ProductModel> NewestProducts { get; set; } = new();

}