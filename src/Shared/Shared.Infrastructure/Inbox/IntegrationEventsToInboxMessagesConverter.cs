using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts;

namespace Shared.Infrastructure.Inbox;

public class IntegrationEventsToInboxMessagesConverter<TEvent, TDbContext> : IConsumer<TEvent>
    where TEvent : IntegrationEvent
    where TDbContext : DbContext
{
    private readonly TDbContext _dbContext;

    public IntegrationEventsToInboxMessagesConverter(TDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task Consume(ConsumeContext<TEvent> context)
    {
        var @event = context.Message;
        var inboxMessage = new InboxMessage(@event.Id, @event.GetType().AssemblyQualifiedName!, @event);
        _dbContext.Set<InboxMessage>().Add(inboxMessage);
        return _dbContext.SaveChangesAsync(context.CancellationToken);
    }
}
