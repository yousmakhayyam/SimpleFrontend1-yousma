using System.ComponentModel.DataAnnotations;
using Warehouse.Domain.Models.Constants;

namespace Warehouse.Web.ViewModels.User;

public class UserRegisterModel
{
    [Required]
    [StringLength(250)]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    [StringLength(250)]
    public string LastName { get; set; } = string.Empty;


    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}