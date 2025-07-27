using System.Text.Json.Serialization;

namespace Ordering.Core.Events;

public record OrderCreated : DomainEvent
{

    public Guid OrderId { get; set; }
    public Guid CustomerId { get; set; }
    public decimal TotalAmount { get; set; }
    public string PaymentMethod { get; set; }
    public IEnumerable<OrderItem> OrderItems { get; set; }

    [JsonConstructor]
    public OrderCreated(
        Guid orderId,
        Guid customerId,
        decimal totalAmount,
        string paymentMethod,
        IEnumerable<OrderItem> orderItems,
        Guid id,
        DateTime occurredOn)
        : base(id, occurredOn)
    {
        OrderId = orderId;
        CustomerId = customerId;
        TotalAmount = totalAmount;
        PaymentMethod = paymentMethod;
        OrderItems = orderItems;
    }

    public OrderCreated(Guid orderId,
                        Guid customerId,
                        decimal totalAmount,
                        string paymentMethod,
                        IEnumerable<OrderItem> orderItems)
    {
        OrderId = orderId;
        CustomerId = customerId;
        TotalAmount = totalAmount;
        PaymentMethod = paymentMethod;
        OrderItems = orderItems;
    }
}
