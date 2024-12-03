using System.ComponentModel.DataAnnotations;

namespace Warehouse.Web.ViewModels.Common;

public class Address
{
    public string AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string City { get; set; }
    public string? State { get; set; }
    public string ZipCode { get; set; }
    public string Country { get; set; }
}