using System.ComponentModel.DataAnnotations;
using Warehouse.Domain.Data.Identity;
using Warehouse.Domain.Models.Common;

namespace Warehouse.Domain.Models.Entities;

public class Company : BaseAuditableEntity
{
    // Primary key
    [Key] public Guid Id { get; set; } = Guid.NewGuid();

    // Basic information fields
    [Required]
    [MaxLength(100)]
    public string Name { get; set; }

    [Required]
    [MaxLength(50)]
    public string TaxIdentificationNumber { get; set; }

    // User ID that created this company
    [Required]
    public string ManagerId { get; set; }

    // List of users associated with the company
    public ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();

    public Guid AddressId { get; set; }
    public Address Address { get; set; }
}