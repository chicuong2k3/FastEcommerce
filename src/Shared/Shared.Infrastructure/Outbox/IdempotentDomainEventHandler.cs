using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Application;
using Shared.Core;

namespace Shared.Infrastructure.Outbox;

internal class IdempotentDomainEventHandler<TEvent, TDbContext> : DomainEventHandler<TEvent>
    where TEvent : DomainEvent
    where TDbContext : DbContext
{
    private readonly TDbContext _dbContext;
    private readonly IDomainEventHandler<TEvent> _innerHandler;
    private readonly ILogger<IdempotentDomainEventHandler<TEvent, TDbContext>> _logger;

    public IDomainEventHandler<TEvent> InnerHandler => _innerHandler;

    public IdempotentDomainEventHandler(
        TDbContext dbContext,
        IDomainEventHandler<TEvent> innerHandler,
        ILogger<IdempotentDomainEventHandler<TEvent, TDbContext>> logger)
    {
        _dbContext = dbContext;
        _innerHandler = innerHandler;
        _logger = logger;
    }

    public override async Task Handle(TEvent domainEvent, CancellationToken cancellationToken)
    {
        var consumer = new OutboxMessageConsumer(domainEvent.Id, _innerHandler.GetType().Name);

        var existingConsumer = await _dbContext.Set<OutboxMessageConsumer>()
            .FirstOrDefaultAsync(x => x.OutboxMessageId == consumer.OutboxMessageId && x.Name == consumer.Name,
                                 cancellationToken);

        if (existingConsumer != null)
        {
            _logger.LogInformation($"Event {domainEvent.Id} already processed.");
            return;
        }


        await _innerHandler.Handle(domainEvent, cancellationToken);

        _dbContext.Set<OutboxMessageConsumer>().Add(consumer);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}