using System.Text.Json.Serialization;

namespace Catalog.Core.Events;

public record ProductVariantAdded : DomainEvent
{
    public Guid ProductId { get; }
    public Guid VariantId { get; }

    [JsonConstructor]
    public ProductVariantAdded(Guid productId, Guid variantId, Guid id, DateTime occurredOn)
        : base(id, occurredOn)
    {
        ProductId = productId;
        VariantId = variantId;
    }

    public ProductVariantAdded(Guid productId, Guid variantId)
    {
        ProductId = productId;
        VariantId = variantId;
    }
}