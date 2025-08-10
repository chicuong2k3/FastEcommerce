namespace PaymentService.Application.Commands;

public record CreatePayment(
    Guid OrderId,
    Guid CustomerId,
    decimal TotalAmount,
    string PaymentProvider) : ICommand;

internal class CreatePaymentHandler(
    IPaymentRepository paymentRepository)
    : ICommandHandler<CreatePayment>
{
    public async Task<Result> Handle(CreatePayment command, CancellationToken cancellationToken)
    {
        var totalAmount = Money.FromDecimal(command.TotalAmount);

        if (!Enum.TryParse<PaymentProvider>(command.PaymentProvider, out var paymentProvider))
            return Result.Fail(new Error($"Invalid payment provider: {command.PaymentProvider}"));

        var payment = new Payment(
                command.OrderId,
                command.CustomerId,
                totalAmount,
                paymentProvider);
        await paymentRepository.AddAsync(payment, cancellationToken);

        return Result.Ok();
    }
}