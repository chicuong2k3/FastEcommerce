
using Catalog.Contracts.ApiClients;
using InventoryService.Core.Repositories;

namespace InventoryService.Application.Features.Stocks;

public record CreateStockCommand(Guid ProductId,
                                 Guid? VariantId,
                                 int AvailableQuantity) : ICommand;


internal sealed class CreateStockCommandHandler(
    IStockRepository stockRepository,
    IProductClient productClient)
    : ICommandHandler<CreateStockCommand>
{
    public async Task<Result> Handle(CreateStockCommand command, CancellationToken cancellationToken)
    {
        var product = await productClient.GetProductByIdAsync(command.ProductId, cancellationToken);
        if (product == null)
        {
            return Result.Fail(new NotFoundError($"The product with id '{command.ProductId}' not found"));
        }

        var variant = product.Variants.FirstOrDefault(v => v.Id == command.VariantId);
        if (variant == null)
        {
            return Result.Fail(new NotFoundError($"The product variant with id '{command.VariantId}' not found"));
        }

        var stock = await stockRepository.GetByProductIdAndVariantIdAsync(command.ProductId, command.VariantId, cancellationToken);
        if (stock != null)
        {
            return Result.Fail($"Stock for product with id '{command.ProductId}' and variant with id '{command.VariantId}' already exists");
        }

        stock = new Stock(command.ProductId, command.VariantId, command.AvailableQuantity);

        await stockRepository.AddAsync(stock);

        return Result.Ok();
    }
}
