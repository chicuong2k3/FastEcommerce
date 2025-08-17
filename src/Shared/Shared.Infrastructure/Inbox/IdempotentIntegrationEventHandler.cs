using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Contracts;

namespace Shared.Infrastructure.Inbox;

internal class IdempotentIntegrationEventHandler<TEvent, TDbContext> : IntegrationEventHandler<TEvent>
    where TEvent : IntegrationEvent
    where TDbContext : DbContext
{
    private readonly TDbContext _dbContext;
    private readonly IIntegrationEventHandler<TEvent> _innerHandler;
    private readonly ILogger<IdempotentIntegrationEventHandler<TEvent, TDbContext>> _logger;

    public IIntegrationEventHandler<TEvent> InnerHandler => _innerHandler;

    public IdempotentIntegrationEventHandler(
        TDbContext dbContext,
        IIntegrationEventHandler<TEvent> innerHandler,
        ILogger<IdempotentIntegrationEventHandler<TEvent, TDbContext>> logger)
    {
        _dbContext = dbContext;
        _innerHandler = innerHandler;
        _logger = logger;
    }

    public override async Task Handle(TEvent integrationEvent, CancellationToken cancellationToken)
    {
        var consumer = new InboxMessageConsumer(integrationEvent.Id, _innerHandler.GetType().Name);

        var existingConsumer = await _dbContext.Set<InboxMessageConsumer>()
            .FirstOrDefaultAsync(x => x.InboxMessageId == consumer.InboxMessageId && x.Name == consumer.Name,
                                 cancellationToken);

        if (existingConsumer != null)
        {
            _logger.LogInformation($"Event {integrationEvent.Id} already processed.");
            return;
        }


        await _innerHandler.Handle(integrationEvent, cancellationToken);

        _dbContext.Set<InboxMessageConsumer>().Add(consumer);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
