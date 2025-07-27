using InventoryService.Application.Mapping;
using InventoryService.Core.Repositories;
using InventoryService.ReadModels;

namespace InventoryService.Application.Features.Stocks;

public record GetStockQuery(Guid ProductId, Guid? ProductVariantId) : IQuery<StockReadModel>;

internal sealed class GetStockQueryHandler(IStockRepository stockRepository)
    : IQueryHandler<GetStockQuery, StockReadModel>
{
    public async Task<Result<StockReadModel>> Handle(GetStockQuery query, CancellationToken cancellationToken)
    {
        var stock = await stockRepository.GetByProductIdAndVariantIdAsync(query.ProductId, query.ProductVariantId, cancellationToken);
        if (stock == null)
        {
            return Result.Fail(new NotFoundError($"Stock for product with ID '{query.ProductId}' not found"));
        }

        return Result.Ok(stock.ToReadModel());
    }
}
