using Microsoft.AspNetCore.Identity;

namespace Warehouse.Domain.Data.Identity;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public Guid? CompanyId { get; set; }
    public DateTimeOffset CreatedOn { get; set; }
    public DateTimeOffset LastUpdateOn { get; set; }
    public string Role { get; set; }

    public string GetFullName => $"{FirstName} {LastName}";
}