namespace Catalog.Requests;

public class GetImagesForProductRequest
{
    public Guid? ProductAttributeId { get; set; }
    public string? ProductAttributeValue { get; set; }
}
