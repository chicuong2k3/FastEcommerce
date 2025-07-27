

namespace Ordering.Core.Entities;

public class Order : AggregateRoot<Guid>
{
    public Guid CustomerId { get; private set; }
    private List<OrderItem> items = [];
    public IReadOnlyCollection<OrderItem> Items => items.AsReadOnly();
    public OrderStatus Status { get; set; }

    public DateTime OrderDate { get; private set; }
    public Money Total { get; private set; }
    public Money Subtotal { get; private set; }
    public Money? Tax { get; private set; }

    public PaymentInfo PaymentInfo { get; set; }
    public ShippingInfo ShippingInfo { get; set; }

    private Order()
    {
        items = new List<OrderItem>();
    }

    private Order(
        Guid customerId,
        PaymentInfo paymentInfo,
        ShippingInfo shippingInfo,
        Money? tax,
        List<OrderItem> orderItems)
    {
        Id = Guid.NewGuid();
        CustomerId = customerId;
        PaymentInfo = paymentInfo;
        ShippingInfo = shippingInfo;
        Tax = tax;
        OrderDate = DateTime.UtcNow;
        items.AddRange(orderItems);
        Subtotal = items.Aggregate(Money.FromDecimal(0), (sum, item) => sum + (item.SalePrice ?? item.BasePrice) * item.Quantity);
        Status = OrderStatus.PendingApproval;
    }

    public static Result<Order> Create(
        Guid customerId,
        PaymentInfo paymentInfo,
        ShippingInfo shippingInfo,
        Money? tax,
        List<OrderItem> orderItems)
    {
        if (!orderItems.Any())
            return Result.Fail("Order must have at least one item");

        var validationResult = paymentInfo.Validate();
        if (validationResult.IsFailed)
            return validationResult;

        validationResult = shippingInfo.Validate();
        if (validationResult.IsFailed)
            return validationResult;


        if (tax != null)
        {
            validationResult = tax.Validate();
            if (validationResult.IsFailed)
                return Result.Fail(validationResult.Errors);
        }

        var order = new Order(customerId, paymentInfo, shippingInfo, tax, orderItems);

        order.Total = order.Subtotal + (tax ?? Money.FromDecimal(0));// + order.ShippingInfo.ShippingCosts;

        order.Raise(new OrderCreated(order.Id, order.CustomerId, order.Total.Amount, order.PaymentInfo.PaymentMethod.ToString(), orderItems));

        return Result.Ok(order);
    }

    public Result MarkAsShipped(string trackingNumber)
    {
        //if (Status != OrderStatus.Preparing)
        //    return Result.Fail(new Error("Order must be Preparing before shipping"));

        Status = OrderStatus.Shipped;
        //Raise(new OrderShipped(Id, trackingNumber));
        return Result.Ok();
    }

    public Result MarkAsDelivered()
    {
        if (Status != OrderStatus.Shipped)
            return Result.Fail(new Error("Order must be Shipped before delivery"));

        Status = OrderStatus.Delivered;
        return Result.Ok();
    }

    public Result Cancel()
    {
        if (Status == OrderStatus.Shipped
            || Status == OrderStatus.Delivered
            || Status == OrderStatus.Canceled
            || Status == OrderStatus.Refunded)
            return Result.Fail(new Error("Cannot cancel a shipped/delivered/canceled/refunded order"));

        Status = OrderStatus.Canceled;
        Raise(new OrderCanceled(Id));
        return Result.Ok();
    }

    public Result Refund(decimal amount)
    {
        if (Status != OrderStatus.Delivered)
            return Result.Fail(new Error("Refunds are only allowed for delivered orders"));

        Status = OrderStatus.Refunded;
        //Raise(new OrderRefunded(Id, amount));
        return Result.Ok();
    }
}
