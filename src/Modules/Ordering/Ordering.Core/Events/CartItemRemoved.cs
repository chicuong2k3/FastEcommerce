using System.Text.Json.Serialization;

namespace Ordering.Core.Events;

public record CartItemRemoved : DomainEvent
{
    public Guid CartId { get; init; }
    public Guid CartItemId { get; init; }
    public CartItemRemoved(Guid cartId, Guid cartItemId)
    {
        CartId = cartId;
        CartItemId = cartItemId;
    }

    [JsonConstructor]
    public CartItemRemoved(Guid cartId, Guid cartItemId, Guid id, DateTime occurredOn)
        : base(id, occurredOn)
    {
        CartId = cartId;
        CartItemId = cartItemId;
    }
}
