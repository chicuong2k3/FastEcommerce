using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Repositories;

namespace Catalog.Infrastructure.Persistence.Repositories;

internal sealed class ProductRepository : RepositoryBase<Product, Guid>, IProductRepository
{
    private readonly CatalogDbContext _dbContext;

    public ProductRepository(CatalogDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products
            .Include(p => p.ProductCategories)
            .Include(p => p.Variants).ThenInclude(v => v.AttributeValues).ThenInclude(av => av.Attribute)
            .Include(p => p.Images)
            .Include(p => p.ProductAttributeValues).ThenInclude(pav => pav.Attribute)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public Task<Product?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default)
    {
        return _dbContext.Products
            .FirstOrDefaultAsync(p => p.Sku == sku, cancellationToken);
    }
}
