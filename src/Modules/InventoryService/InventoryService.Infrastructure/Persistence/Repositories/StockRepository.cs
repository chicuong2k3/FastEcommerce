using InventoryService.Core.Entities;
using InventoryService.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Repositories;

namespace InventoryService.Infrastructure.Persistence.Repositories;

internal class StockRepository : RepositoryBase<Stock, Guid>, IStockRepository
{
    private readonly InventoryDbContext _dbContext;

    public StockRepository(InventoryDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Stock?> GetByProductIdAndVariantIdAsync(Guid productId, Guid? variantId = null, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Stocks
            .FirstOrDefaultAsync(s => s.ProductId == productId && s.VariantId == variantId, cancellationToken);
    }
}
