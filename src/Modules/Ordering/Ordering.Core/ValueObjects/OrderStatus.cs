namespace Ordering.Core.ValueObjects;

public enum OrderStatus
{
    PendingApproval,
    PendingPayment, // Awaiting payment, the initial state when the order is created
    Paid,
    Shipped,        // Handed to logistics
    Delivered,      // Successfully delivered
    Canceled,       // Abandoned before fulfillment
    Refunded        // Refund issued post-delivery
}
