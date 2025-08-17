//using Microsoft.Extensions.Logging;
//using MediatR;
//using Ordering.Contracts;
//using PaymentService.Core.PaymentGateways;
//using PaymentService.Application.Commands;
//using Shared.Infrastructure.Inbox;
//using PaymentService.Core.Repositories;

//namespace PaymentService.Application.EventHandlers.IntegrationEvents;

//internal class ProcessPaymentOnOrderCreated
//    : IntegrationEventHandler<OrderCreatedIntegrationEvent>
//{
//    private readonly ILogger<ProcessPaymentOnOrderCreated> _logger;
//    private readonly IPaymentGatewayFactory _paymentGatewayFactory;
//    private readonly IMediator _mediator;
//    private readonly IPaymentRepository _paymentRepository;

//    public ProcessPaymentOnOrderCreated(
//        ILogger<ProcessPaymentOnOrderCreated> logger,
//        IPaymentGatewayFactory paymentGatewayFactory,
//        IMediator mediator,
//        IPaymentRepository paymentRepository)
//    {
//        _logger = logger;
//        _paymentGatewayFactory = paymentGatewayFactory;
//        _mediator = mediator;
//        _paymentRepository = paymentRepository;
//    }

//    public override async Task Handle(OrderCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
//    {
//        var createPaymentResult = await _mediator.Send(
//            new CreatePayment(integrationEvent.OrderId, integrationEvent.CustomerId, integrationEvent.TotalAmount, integrationEvent.PaymentProvider),
//            cancellationToken);

//        if (createPaymentResult.IsFailed)
//        {
//            _logger.LogError("Failed to create payment for order {OrderId}: {Errors}",
//                integrationEvent.OrderId, string.Join(", ", createPaymentResult.Errors));
//            throw new Exception("Payment creation failed");
//        }


//        var paymentProvider = Enum.Parse<PaymentProvider>(integrationEvent.PaymentProvider);
//        var gateway = _paymentGatewayFactory.CreateGateway(paymentProvider);
//        var payment = await _paymentRepository.GetPaymentByOrderIdAsync(integrationEvent.OrderId, cancellationToken);

//        if (payment == null)
//        {
//            _logger.LogError("Payment not found for order {OrderId}", integrationEvent.OrderId);
//            throw new Exception("Payment not found");
//        }

//        var paymentUrlResult = gateway.CreatePaymentUrl(payment);

//        if (paymentUrlResult.IsFailed)
//        {
//            _logger.LogError("Failed to create payment URL for order {OrderId}: {Errors}",
//                integrationEvent.OrderId, string.Join(", ", paymentUrlResult.Errors));
//            throw new Exception("Payment URL creation failed");
//        }

//        payment.SetPaymentUrlAndToken(
//            paymentUrlResult.Value.PaymentUrl,
//            paymentUrlResult.Value.PaymentToken);

//        await _paymentRepository.SaveChangesAsync(cancellationToken);

//        _logger.LogInformation(
//                        "Payment URL generated for order {OrderId}: {PaymentUrl}",
//                        integrationEvent.OrderId,
//                        paymentUrlResult.Value.PaymentUrl);

//    }
//}
