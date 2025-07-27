namespace Ordering.Application.Orders;

public record CancelOrderCommand(Guid OrderId) : ICommand;

public class CancelOrderCommandHandler(IOrderRepository orderRepository)
    : ICommandHandler<CancelOrderCommand>
{
    public async Task<Result> Handle(CancelOrderCommand command, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetByIdAsync(command.OrderId);
        if (order == null)
            return Result.Fail(new NotFoundError($"Order with id '{command.OrderId}' not found"));

        var result = order.Cancel();

        if (result.IsFailed)
            return result;

        await orderRepository.SaveChangesAsync(cancellationToken);
        return result;
    }
}