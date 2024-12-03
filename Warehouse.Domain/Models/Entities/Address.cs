using System.ComponentModel.DataAnnotations;

namespace Warehouse.Domain.Models.Entities;

public class Address
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(100)]
    public string AddressLine1 { get; set; }

    [MaxLength(100)]
    public string? AddressLine2 { get; set; }

    [Required]
    [MaxLength(50)]
    public string City { get; set; }

    [MaxLength(50)]
    public string? State { get; set; }

    [Required]
    [MaxLength(10)]
    public string ZipCode { get; set; }

    [Required]
    [MaxLength(50)]
    public string Country { get; set; }
}