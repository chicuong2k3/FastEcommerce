using System.Text.Json.Serialization;

namespace Catalog.Core.Events;

public record ProductImageAdded : DomainEvent
{
    public Guid ProductId { get; }
    public Guid ProductImageId { get; set; }

    [JsonConstructor]
    public ProductImageAdded(Guid productId, Guid productImageId, Guid id, DateTime occurredOn)
        : base(id, occurredOn)
    {
        ProductId = productId;
        ProductImageId = productImageId;
    }

    public ProductImageAdded(Guid productId, Guid productImageId)
    {
        ProductId = productId;
        ProductImageId = productImageId;
    }
}