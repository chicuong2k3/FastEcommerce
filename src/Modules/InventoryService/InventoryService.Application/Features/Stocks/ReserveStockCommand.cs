using InventoryService.Core.Repositories;

namespace InventoryService.Application.Features.Stocks;

public record ReserveStockCommand(
    Guid ProductId,
    Guid? VariantId,
    int Quantity) : ICommand;


internal sealed class ReserveStockCommandHandler(IStockRepository stockRepository)
    : ICommandHandler<ReserveStockCommand>
{
    public async Task<Result> Handle(ReserveStockCommand command, CancellationToken cancellationToken)
    {
        var stock = await stockRepository.GetByProductIdAndVariantIdAsync(
            command.ProductId,
            command.VariantId,
            cancellationToken);
        if (stock == null)
        {
            return Result.Fail(new NotFoundError($"The stock item associated with ProductId '{command.ProductId}' not found"));
        }

        var result = stock.Reserve(command.Quantity);

        await stockRepository.SaveChangesAsync(cancellationToken);
        return result;
    }
}

