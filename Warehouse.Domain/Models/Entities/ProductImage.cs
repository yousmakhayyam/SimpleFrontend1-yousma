using System.ComponentModel.DataAnnotations;

namespace Warehouse.Domain.Models.Entities;

public class ProductImage
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid FileId { get; set; } // ID of the image file

    [Required]
    [MaxLength(250)]
    public string FileName { get; set; }

    public bool IsDefault { get; set; } // Indicates if this is the default image for the product

    // Foreign key to associate with Product
    public Guid ProductId { get; set; }
    public Product Product { get; set; }

    public string GetFullFileName => $"{FileId}_{FileName}";
}