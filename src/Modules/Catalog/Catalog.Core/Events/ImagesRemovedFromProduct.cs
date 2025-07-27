using System.Text.Json.Serialization;

namespace Catalog.Core.Events;

public record ImagesRemovedFromProduct
    : DomainEvent
{
    public Guid ProductId { get; set; }
    public List<string> ImageUrls { get; set; } = [];

    [JsonConstructor]
    public ImagesRemovedFromProduct(Guid productId, List<string> imageUrls, Guid id, DateTime occurredOn)
        : base(id, occurredOn)
    {
        ProductId = productId;
        ImageUrls = imageUrls;
    }

    public ImagesRemovedFromProduct(Guid productId, List<string> imageUrls)
    {
        ProductId = productId;
        ImageUrls = imageUrls;
    }
}
