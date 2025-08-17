using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Repositories;

namespace Catalog.Infrastructure.Persistence.Repositories;

internal class BrandRepository : RepositoryBase<Brand, Guid>, IBrandRepository
{
    private readonly CatalogDbContext _dbContext;

    public BrandRepository(CatalogDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<Brand>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Brands.ToListAsync(cancellationToken);
    }

    public async Task<Brand?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Brands
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public async Task<Brand?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Brands
            .FirstOrDefaultAsync(b => b.Name.ToLower() == name.ToLower(), cancellationToken);
    }
}
