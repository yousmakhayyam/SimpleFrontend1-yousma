using System.ComponentModel.DataAnnotations;

namespace Warehouse.Web.ViewModels.Company;

public class RegisterCompanyModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [StringLength(250)]
    public string ManagerFirstName { get; set; }

    [Required]
    [StringLength(250)]
    public string ManagerLastName { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }

    [Required]
    [StringLength(250)]
    public string Name { get; set; }

    [Required]
    [StringLength(25)]
    public string TaxIdentificationNumber { get; set; }

    [Required]
    [StringLength(250)] 
    public string AddressLine1 { get; set; }
    public string AddressLine2 { get; set; }

    [Required]
    [StringLength(250)]
    public string City { get; set; }

    [Required]
    [StringLength(250)]
    public string Country { get; set; }
    public string State { get; set; }
    
    [Required]
    [StringLength(250)]
    public string ZipCode { get; set; }

    public string UserId { get; set; }
}