namespace PaymentService.Core.ValueObjects;

public enum PaymentStatus
{
    Pending,
    UrlGenerated,
    Succeeded,
    Failed,
    Canceled,
    Refunded,
    Expired
}
