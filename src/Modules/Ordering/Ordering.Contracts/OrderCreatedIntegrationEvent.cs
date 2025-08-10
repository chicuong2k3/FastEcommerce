using Shared.Contracts;
using System.Text.Json.Serialization;

namespace Ordering.Contracts;

public class OrderCreatedIntegrationEvent : IntegrationEvent
{
    public OrderCreatedIntegrationEvent(
        Guid orderId,
        Guid customerId,
        decimal totalAmount)
    {
        OrderId = orderId;
        CustomerId = customerId;
        TotalAmount = totalAmount;
    }

    [JsonConstructor]
    public OrderCreatedIntegrationEvent(
        Guid orderId,
        Guid customerId,
        decimal totalAmount,
        Guid id,
        DateTime occurredOn) : base(id, occurredOn)
    {
        OrderId = orderId;
        CustomerId = customerId;
        TotalAmount = totalAmount;
    }

    public Guid OrderId { get; }
    public Guid CustomerId { get; }
    public decimal TotalAmount { get; set; }
}
