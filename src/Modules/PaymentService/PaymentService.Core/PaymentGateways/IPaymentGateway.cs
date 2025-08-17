namespace PaymentService.Core.PaymentGateways;

public interface IPaymentGateway
{
    string Reference { get; }

    Result<PaymentUrlInfo> CreatePaymentUrl(
        Payment payment);

    Task<Result> RefundAsync(
        Payment payment,
        decimal refundAmount,
        string refundReason,
        CancellationToken cancellationToken = default);
}
