using System.Text.Json.Serialization;

namespace Ordering.Core.Events;

public record CartItemAdded : DomainEvent
{
    public Guid CartId { get; init; }
    public Guid CartItemId { get; init; }
    public CartItemAdded(Guid cartId, Guid cartItemId)
    {
        CartId = cartId;
        CartItemId = cartItemId;
    }

    [JsonConstructor]
    public CartItemAdded(Guid cartId, Guid cartItemId, Guid id, DateTime occurredOn)
        : base(id, occurredOn)
    {
        CartId = cartId;
        CartItemId = cartItemId;
    }
}