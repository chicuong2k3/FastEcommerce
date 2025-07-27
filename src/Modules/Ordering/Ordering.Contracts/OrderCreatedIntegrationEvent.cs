using Shared.Contracts;
using System.Text.Json.Serialization;

namespace Ordering.Contracts;

public class OrderCreatedIntegrationEvent : IntegrationEvent
{
    public OrderCreatedIntegrationEvent(
        Guid orderId,
        Guid customerId)
    {
        OrderId = orderId;
        CustomerId = customerId;
    }

    [JsonConstructor]
    public OrderCreatedIntegrationEvent(
        Guid orderId,
        Guid customerId,
        Guid id,
        DateTime occurredOn) : base(id, occurredOn)
    {
        OrderId = orderId;
        CustomerId = customerId;
    }

    public Guid OrderId { get; }
    public Guid CustomerId { get; }
}
