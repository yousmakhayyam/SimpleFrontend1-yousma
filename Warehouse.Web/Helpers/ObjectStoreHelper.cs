namespace Warehouse.Web.Helpers;

public class ObjectStoreHelper
{
    public readonly string ProductImageDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "images");

    public ObjectStoreHelper()
    {
        if (!Directory.Exists(ProductImageDirectory))
        {
            Directory.CreateDirectory(ProductImageDirectory);
        }
    }

    public async Task Create(Stream fileStream, string fileName)
    {
        var filePath = Path.Combine(ProductImageDirectory, fileName);

        // Save the file to the specified location
        await using var outputStream = new FileStream(filePath, FileMode.Create);
        await fileStream.CopyToAsync(outputStream);
    }
}