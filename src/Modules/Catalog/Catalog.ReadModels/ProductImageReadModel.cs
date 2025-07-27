namespace Catalog.ReadModels;

public class ProductImageReadModel
{
    public Guid Id { get; set; } = default!;
    public string Url { get; set; }
    public string? AltText { get; set; }
    public bool IsThumbnail { get; set; }
    public int SortOrder { get; set; }
    public Guid? ProductAttributeId { get; set; }
    public string? ProductAttributeValue { get; set; }
    public Guid ProductId { get; set; }
}
