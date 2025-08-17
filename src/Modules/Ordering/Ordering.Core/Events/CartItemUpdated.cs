using System.Text.Json.Serialization;

namespace Ordering.Core.Events;

public record CartItemUpdated : DomainEvent
{
    public Guid CartId { get; }
    public Guid CartItemId { get; }
    public CartItemUpdated(Guid cartId, Guid cartItemId)
    {
        CartId = cartId;
        CartItemId = cartItemId;
    }

    [JsonConstructor]
    public CartItemUpdated(Guid cartId, Guid cartItemId, Guid id, DateTime occurredOn)
        : base(id, occurredOn)
    {
        CartId = cartId;
        CartItemId = cartItemId;
    }
}