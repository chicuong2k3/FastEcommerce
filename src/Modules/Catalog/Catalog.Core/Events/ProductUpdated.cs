using System.Text.Json.Serialization;

namespace Catalog.Core.Events;

public record ProductUpdated : DomainEvent
{
    public Guid ProductId { get; }

    [JsonConstructor]
    public ProductUpdated(Guid productId, Guid id, DateTime occurredOn)
        : base(id, occurredOn)
    {
        ProductId = productId;
    }

    public ProductUpdated(Guid productId)
    {
        ProductId = productId;
    }
}
