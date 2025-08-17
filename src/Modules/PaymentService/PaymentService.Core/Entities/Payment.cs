namespace PaymentService.Core.Entities;

public class Payment : AggregateRoot<Guid>
{
    public Guid OrderId { get; private set; }
    public Guid CustomerId { get; private set; }
    public Money TotalAmount { get; private set; }
    public PaymentStatus Status { get; private set; }
    public PaymentProvider PaymentProvider { get; private set; }
    public string? PaymentUrl { get; private set; }
    public string? PaymentToken { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? PaidAt { get; private set; }

    public string? ProviderTransactionId { get; private set; }

    private Payment() { }

    public Payment(
        Guid orderId,
        Guid customerId,
        Money totalAmount,
        PaymentProvider paymentProvider)
    {
        Id = Guid.NewGuid();
        OrderId = orderId;
        CustomerId = customerId;
        TotalAmount = totalAmount;
        Status = PaymentStatus.Pending;
        PaymentProvider = paymentProvider;
        CreatedAt = DateTime.UtcNow;
    }

    public void SetPaymentUrlAndToken(string paymentUrl, string paymentToken)
    {
        if (Status != PaymentStatus.Pending)
            throw new InvalidOperationException("Can only set payment URL for pending payments");

        PaymentUrl = paymentUrl;
        PaymentToken = paymentToken;
        Status = PaymentStatus.UrlGenerated;
    }

    public Result ProcessCallback(string providerTransactionId, PaymentResponseCode responseCode)
    {
        // Validate payment status
        if (Status != PaymentStatus.UrlGenerated && Status != PaymentStatus.Pending)
            return Result.Fail("Invalid payment status for callback processing");

        if (CreatedAt.AddMinutes(30) < DateTime.UtcNow)
        {
            Status = PaymentStatus.Expired;
            return Result.Fail("Payment URL has expired");
        }

        ProviderTransactionId = providerTransactionId;

        switch (responseCode)
        {
            case PaymentResponseCode.Success:
                Status = PaymentStatus.Succeeded;
                PaidAt = DateTime.UtcNow;
                Raise(new PaymentSucceeded(OrderId));
                break;

            case PaymentResponseCode.Canceled:
                Status = PaymentStatus.Canceled;
                Raise(new PaymentCanceled(OrderId));
                break;

            case PaymentResponseCode.Failed:
                Status = PaymentStatus.Failed;
                Raise(new PaymentFailed(OrderId));
                break;
        }

        return Result.Ok();
    }

    public Result Cancel()
    {
        if (Status != PaymentStatus.Pending && Status != PaymentStatus.UrlGenerated)
            return Result.Fail("Can only cancel pending or URL generated payments");

        Status = PaymentStatus.Canceled;
        Raise(new PaymentCanceled(OrderId));
        return Result.Ok();
    }

    public async Task<Result> RefundAsync(
        IPaymentGateway gateway,
        decimal refundAmount,
        string reason,
        CancellationToken cancellationToken = default)
    {
        if (Status != PaymentStatus.Succeeded)
            return Result.Fail("Only succeeded payments can be refunded");

        if (refundAmount <= 0)
            return Result.Fail("Refund amount must be greater than zero");

        if (refundAmount > TotalAmount.Amount)
            return Result.Fail("Refund amount cannot exceed payment amount");

        var result = await gateway.RefundAsync(this, refundAmount, reason, cancellationToken);
        if (result.IsSuccess)
        {
            Status = PaymentStatus.Refunded;
            Raise(new PaymentRefunded(Id, refundAmount, reason));
        }

        return result;
    }
}
