﻿using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Repositories;

namespace Catalog.Infrastructure.Persistence.Repositories;

internal class ProductAttributeRepository : RepositoryBase<ProductAttribute, Guid>, IProductAttributeRepository
{
    private readonly CatalogDbContext _dbContext;

    public ProductAttributeRepository(CatalogDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddValue(ProductAttributeValue productAttributeValue, CancellationToken cancellationToken = default)
    {
        _dbContext.ProductAttributeValues.Add(productAttributeValue);
        await _dbContext.SaveChangesAsync();
    }

    public Task<List<ProductAttribute>> GetAttributesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.ProductAttributes
            .ToListAsync(cancellationToken);
    }

    public async Task<ProductAttribute?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ProductAttributes
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<List<ProductAttribute>> GetByIdsAsync(List<Guid> ids, CancellationToken cancellationToken = default)
    {
        var attributes = await _dbContext.ProductAttributes.Where(a => ids.Contains(a.Id)).ToListAsync();
        return attributes;
    }

    public async Task<ProductAttribute?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ProductAttributes
            .FirstOrDefaultAsync(a => a.Name.ToLower() == name.ToLower(), cancellationToken);
    }

    public async Task<List<ProductAttributeValue>> GetValuesAsync(Guid productAttributeId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ProductAttributeValues
            .Where(x => x.AttributeId == productAttributeId)
            .Include(x => x.Attribute)
            .ToListAsync(cancellationToken);
    }
}
