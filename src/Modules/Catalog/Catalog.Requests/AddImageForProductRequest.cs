
namespace Catalog.Requests;

public class AddImageForProductRequest
{
    public Guid? ProductAttributeId { get; set; }
    public string? ProductAttributeValue { get; set; }
    public string ImageUrl { get; set; }
    public string? ImageAltText { get; set; }
    public bool IsThumbnail { get; set; }
    public int SortOrder { get; set; }
}
