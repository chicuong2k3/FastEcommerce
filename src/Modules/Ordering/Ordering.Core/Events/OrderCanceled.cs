using System.Text.Json.Serialization;

namespace Ordering.Core.Events;

public record OrderCanceled : DomainEvent
{
    public Guid OrderId { get; set; }

    public OrderCanceled() { }
    [JsonConstructor]
    public OrderCanceled(Guid orderId, Guid id, DateTime occurredOn)
        : base(id, occurredOn)
    {
        OrderId = orderId;
    }

    public OrderCanceled(Guid orderId)
    {
        OrderId = orderId;
    }
}
