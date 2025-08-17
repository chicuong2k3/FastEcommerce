using Microsoft.EntityFrameworkCore.Diagnostics;
using Shared.Core;

namespace Shared.Infrastructure.Outbox;

public sealed class DomainEventsToOutboxMessagesInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        ProcessDomainEvents(eventData);
        return result;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        ProcessDomainEvents(eventData);
        return ValueTask.FromResult(result);
    }

    private void ProcessDomainEvents(DbContextEventData eventData)
    {
        var dbContext = eventData.Context;
        if (dbContext == null)
        {
            return;
        }

        var entities = dbContext.ChangeTracker
            .Entries<IAggregateRoot>()
            .Select(x => x.Entity)
            .ToList();

        var outboxMessages = entities
            .SelectMany(x =>
            {
                var domainEvents = x.GetDomainEvents();
                return domainEvents;
            })
            .Select(e => new OutboxMessage
            (
                e.Id,
                e.GetType().AssemblyQualifiedName!,
                e
            ))
            .ToList();

        foreach (var entity in entities)
        {
            entity.ClearDomainEvents();
        }

        if (outboxMessages.Any())
        {
            dbContext.Set<OutboxMessage>().AddRange(outboxMessages);
        }
    }
}