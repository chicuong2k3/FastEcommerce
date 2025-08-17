using Catalog.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Infrastructure.Inbox;
using Shared.Infrastructure.Outbox;

namespace Catalog.Infrastructure.Persistence;

public sealed class CatalogDbContext : DbContext
{
    public CatalogDbContext(
        DbContextOptions<CatalogDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("catalog");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogDbContext).Assembly);
    }

    public DbSet<Category> Categories { get; set; } = default!;
    public DbSet<Product> Products { get; set; } = default!;
    public DbSet<ProductVariant> ProductVariants { get; set; } = default!;
    public DbSet<ProductAttribute> ProductAttributes { get; set; } = default!;
    public DbSet<ProductAttributeValue> ProductAttributeValues { get; set; } = default!;
    public DbSet<Brand> Brands { get; set; } = default!;

    public DbSet<OutboxMessage> OutboxMessages { get; set; } = default!;
    public DbSet<OutboxMessageConsumer> OutboxMessageConsumers { get; set; } = default!;
    public DbSet<InboxMessage> InboxMessages { get; set; } = default!;
    public DbSet<InboxMessageConsumer> InboxMessageConsumers { get; set; } = default!;
}
