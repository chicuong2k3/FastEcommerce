using PaymentService.Core.ValueObjects;

namespace PaymentService.Core.PaymentGateways;

public interface IPaymentGatewayFactory
{
    IPaymentGateway CreateGateway(PaymentProvider paymentProvider);
}