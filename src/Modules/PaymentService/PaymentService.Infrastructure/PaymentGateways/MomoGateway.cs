using FluentResults;
using PaymentService.Core.Entities;
using PaymentService.Core.PaymentGateways;
using PaymentService.Core.ValueObjects;

namespace PaymentService.Infrastructure.PaymentGateways;

internal class MomoGateway : IPaymentGateway
{
    public string Reference => "MOMO_GATEWAY";

    public Result<PaymentUrlInfo> CreatePaymentUrl(Payment payment)
    {
        throw new NotImplementedException();
    }

    public Task<Result> RefundAsync(Payment payment, decimal refundAmount, string refundReason, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
