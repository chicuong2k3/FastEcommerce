using InventoryService.Core.Repositories;

namespace InventoryService.Application.Features.Stocks;

public record ReleaseStockCommand(
    Guid ProductId,
    Guid? VariantId,
    int Quantity) : ICommand;

internal sealed class ReleaseStockCommandHandler(IStockRepository stockRepository)
    : ICommandHandler<ReleaseStockCommand>
{
    public async Task<Result> Handle(ReleaseStockCommand command, CancellationToken cancellationToken)
    {
        var stock = await stockRepository.GetByProductIdAndVariantIdAsync(
            command.ProductId,
            command.VariantId,
            cancellationToken);

        if (stock == null)
        {
            return Result.Fail(new NotFoundError($"The stock item associated with ProductId '{command.ProductId}' not found"));
        }

        var result = stock.Release(command.Quantity);

        await stockRepository.SaveChangesAsync(cancellationToken);
        return result;
    }
}
