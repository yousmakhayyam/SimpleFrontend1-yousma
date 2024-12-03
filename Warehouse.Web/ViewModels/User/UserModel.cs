namespace Warehouse.Web.ViewModels.User;

public class UserModel
{
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public string PhoneNumber { get; set; }
    public DateTimeOffset JoinedOn { get; set; }
    public Guid? CompanyId { get; set; }
    public string? Company { get; set; }

    public string FullName => $"{FirstName} {LastName}";
}