namespace Catalog.Core.Entities;

public class ProductImage : Entity<Guid>
{
    public string Url { get; private set; }
    public string? AltText { get; private set; }
    public bool IsThumbnail { get; private set; }
    public int SortOrder { get; private set; }

    public Guid? ProductAttributeId { get; private set; }
    public string? ProductAttributeValue { get; private set; }

    public Guid ProductId { get; set; }

    private ProductImage()
    {
    }

    public ProductImage(
        string url,
        string? altText,
        bool isThumbnail,
        int sortOrder,
        Guid? productAttributeId,
        string? productAttributeValue)
    {
        Id = Guid.NewGuid();
        Url = url;
        AltText = altText;
        IsThumbnail = isThumbnail;
        SortOrder = sortOrder;
        ProductAttributeId = productAttributeId;
        ProductAttributeValue = productAttributeValue;
    }
}
