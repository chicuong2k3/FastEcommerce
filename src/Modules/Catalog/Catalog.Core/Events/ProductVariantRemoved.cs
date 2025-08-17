using System.Text.Json.Serialization;

namespace Catalog.Core.Events;

public record ProductVariantRemoved : DomainEvent
{
    public Guid ProductId { get; }
    public Guid VariantId { get; }

    [JsonConstructor]
    public ProductVariantRemoved(Guid productId, Guid variantId, Guid id, DateTime occurredOn)
        : base(id, occurredOn)
    {
        ProductId = productId;
        VariantId = variantId;
    }

    public ProductVariantRemoved(Guid productId, Guid variantId)
    {
        ProductId = productId;
        VariantId = variantId;
    }
}
