using Shared.Core;

namespace Shared.Application;

public abstract class DomainEventHandler<TEvent> : IDomainEventHandler<TEvent>
    where TEvent : DomainEvent
{
    public abstract Task Handle(TEvent domainEvent, CancellationToken cancellationToken = default);

    public Task Handle(DomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        if (domainEvent is not TEvent specificDomainEvent)
        {
            throw new ArgumentException($"Invalid domain event type: {domainEvent.GetType().Name}");
        }

        return Handle(specificDomainEvent, cancellationToken);
    }
}
