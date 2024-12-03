using System.ComponentModel.DataAnnotations;
using Warehouse.Domain.Models.Constants;

namespace Warehouse.Web.ViewModels.User;

public class UserEditModel
{
    public string? Id { get; set; } // Null for new users

    [Required]
    [StringLength(250)]
    public string FirstName { get; set; }

    [Required]
    [StringLength(250)]
    public string LastName { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    public string Password { get; set; } // For new users only

    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; } // For new users only

    public string Role { get; set; } = Roles.User;
    public Guid? CompanyId { get; set; } // Null for admin
}