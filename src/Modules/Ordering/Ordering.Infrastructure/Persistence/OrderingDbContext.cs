using Microsoft.EntityFrameworkCore;
using Ordering.Core.Entities;
using Shared.Infrastructure.Inbox;
using Shared.Infrastructure.Outbox;

namespace Ordering.Infrastructure.Persistence;

public class OrderingDbContext : DbContext
{
    public OrderingDbContext(
        DbContextOptions<OrderingDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("ordering");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderingDbContext).Assembly);
    }

    public DbSet<Order> Orders { get; set; }
    public DbSet<Cart> Carts { get; set; }

    public DbSet<OutboxMessage> OutboxMessages { get; set; } = default!;
    public DbSet<OutboxMessageConsumer> OutboxMessageConsumers { get; set; } = default!;
    public DbSet<InboxMessage> InboxMessages { get; set; } = default!;
    public DbSet<InboxMessageConsumer> InboxMessageConsumers { get; set; } = default!;

}
