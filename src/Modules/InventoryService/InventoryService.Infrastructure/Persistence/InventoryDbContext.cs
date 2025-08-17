using InventoryService.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Inbox;
using Shared.Infrastructure.Outbox;

namespace InventoryService.Infrastructure.Persistence;

public class InventoryDbContext : DbContext
{
    public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("inventory");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(InventoryDbContext).Assembly);
    }

    public DbSet<Stock> Stocks { get; set; }

    public DbSet<OutboxMessage> OutboxMessages { get; set; } = default!;
    public DbSet<OutboxMessageConsumer> OutboxMessageConsumers { get; set; } = default!;
    public DbSet<InboxMessage> InboxMessages { get; set; } = default!;
    public DbSet<InboxMessageConsumer> InboxMessageConsumers { get; set; } = default!;
}
