using System.Text.Json.Serialization;

namespace Catalog.Core.Events;

public record ProductDeleted : DomainEvent
{
    public Guid ProductId { get; }
    public List<string> ImageUrls { get; } = new List<string>();

    [JsonConstructor]
    public ProductDeleted(Guid productId, List<string> imageUrls, Guid id, DateTime occurredOn)
        : base(id, occurredOn)
    {
        ProductId = productId;
    }

    public ProductDeleted(Guid productId, List<string> imageUrls)
    {
        ProductId = productId;
        ImageUrls = imageUrls;
    }
}
