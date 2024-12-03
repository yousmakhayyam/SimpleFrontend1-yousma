using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Warehouse.Domain.Models.Common;

namespace Warehouse.Domain.Models.Entities;

public class Product : BaseAuditableEntity
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(250)]
    public string Name { get; set; }

    public string Description { get; set; }

    // Images associated with the product
    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();

    // Ownership properties
    public string OwnerId { get; set; }
    public Guid? CreatedByCompanyId { get; set; }

    // Additional properties
    [Required]
    public Money Price { get; set; }

    public int StockQuantity { get; set; }

    [MaxLength(50)]
    public string Category { get; set; }

    // Dictionary to hold dynamic properties
    [JsonIgnore] // Optional: prevent JSON serialization if using ASP.NET API serialization
    public Dictionary<string, string> DynamicProperties { get; set; } = new Dictionary<string, string>();

    // Helper method to add a dynamic property
    public void AddDynamicProperty(string key, string value)
    {
        if (!DynamicProperties.ContainsKey(key))
        {
            DynamicProperties.Add(key, value);
        }
        else
        {
            DynamicProperties[key] = value; // Update value if key exists
        }
    }

    // Helper method to remove a dynamic property
    public void RemoveDynamicProperty(string key)
    {
        DynamicProperties.Remove(key);
    }

    // Helper method to get a dynamic property by key
    public string GetDynamicProperty(string key)
    {
        return DynamicProperties.TryGetValue(key, out var value) ? value : null;
    }
}