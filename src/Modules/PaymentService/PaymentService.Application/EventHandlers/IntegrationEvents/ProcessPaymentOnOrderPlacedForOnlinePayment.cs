//using Microsoft.Extensions.Logging;
//using MediatR;
//using Ordering.Contracts;
//using PaymentService.Core.PaymentGateways;
//using PaymentService.Application.Commands;

//namespace PaymentService.Application.EventHandlers.IntegrationEvents;

//internal class ProcessPaymentOnOrderPlacedForOnlinePayment
//    : IntegrationEventHandler<OrderPlacedForOnlinePaymentIntegrationEvent>
//{
//    private readonly ILogger<ProcessPaymentOnOrderPlacedForOnlinePayment> logger;
//    private readonly IPaymentGatewayFactory paymentGatewayFactory;
//    private readonly IMediator mediator;
//    private readonly IPaymentRepository paymentRepository;

//    public ProcessPaymentOnOrderPlacedForOnlinePayment(
//        ILogger<ProcessPaymentOnOrderPlacedForOnlinePayment> logger,
//        IPaymentGatewayFactory paymentGatewayFactory,
//        IMediator mediator,
//        IPaymentRepository paymentRepository)
//    {
//        this.logger = logger;
//        this.paymentGatewayFactory = paymentGatewayFactory;
//        this.mediator = mediator;
//        this.paymentRepository = paymentRepository;
//    }

//    public override async Task Handle(OrderPlacedForOnlinePaymentIntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
//    {
//        try
//        {
//            var createPaymentResult = await mediator.Send(
//                new CreatePayment(Guid.NewGuid(), integrationEvent.OrderId, integrationEvent.CustomerId, integrationEvent.TotalAmount, integrationEvent.PaymentProvider),
//                cancellationToken);

//            if (createPaymentResult.IsFailed)
//            {
//                logger.LogError("Failed to create payment for order {OrderId}: {Errors}",
//                    integrationEvent.OrderId, string.Join(", ", createPaymentResult.Errors));
//                throw new Exception("Payment creation failed");
//            }


//            var paymentProvider = Enum.Parse<PaymentProvider>(integrationEvent.PaymentProvider);
//            var gateway = paymentGatewayFactory.CreateGateway(paymentProvider);
//            var payment = await paymentRepository.GetPaymentByOrderIdAsync(integrationEvent.OrderId, cancellationToken);

//            if (payment == null)
//            {
//                logger.LogError("Payment not found for order {OrderId}", integrationEvent.OrderId);
//                throw new Exception("Payment not found");
//            }

//            var paymentUrlResult = gateway.CreatePaymentUrl(payment);

//            if (paymentUrlResult.IsFailed)
//            {
//                logger.LogError("Failed to create payment URL for order {OrderId}: {Errors}",
//                    integrationEvent.OrderId, string.Join(", ", paymentUrlResult.Errors));
//                throw new Exception("Payment URL creation failed");
//            }

//            payment.SetPaymentUrlAndToken(
//                paymentUrlResult.Value.PaymentUrl,
//                paymentUrlResult.Value.PaymentToken);

//            await paymentRepository.SaveChangesAsync(cancellationToken);

//            logger.LogInformation(
//                            "Payment URL generated for order {OrderId}: {PaymentUrl}",
//                            integrationEvent.OrderId,
//                            paymentUrlResult.Value.PaymentUrl);
//        }
//        catch (Exception ex)
//        {
//            logger.LogError(ex, "Error processing payment for order {OrderId}", integrationEvent.OrderId);
//            throw;
//        }
//    }
//}
