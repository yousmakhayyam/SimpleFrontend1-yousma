using Warehouse.Web.ViewModels.Products;

namespace Warehouse.Web.ViewModels.User;

public class ProfileModel : UserModel
{
    public List<ProductModel> NewestProducts { get; set; } = new(); 
}