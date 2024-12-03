namespace Warehouse.Domain.Models.Common;

public class BaseAuditableEntity
{
    public DateTimeOffset Created { get; set; }

    public string? CreatedBy { get; set; }

    public DateTimeOffset LastModified { get; set; }

    public string? LastModifiedBy { get; set; }

    public void FillCreated(string userId)
    {
        Created = DateTimeOffset.Now;
        CreatedBy = userId;
    }

    public void FillModified(string userId)
    {
        Created = DateTimeOffset.Now;
        CreatedBy = userId;
    }
}