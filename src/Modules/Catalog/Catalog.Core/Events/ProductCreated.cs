using System.Text.Json.Serialization;

namespace Catalog.Core.Events;

public record ProductCreated : DomainEvent
{
    public Guid ProductId { get; }

    [JsonConstructor]
    public ProductCreated(Guid productId, Guid id, DateTime occurredOn)
        : base(id, occurredOn)
    {
        ProductId = productId;
    }

    public ProductCreated(Guid productId)
    {
        ProductId = productId;
    }
}