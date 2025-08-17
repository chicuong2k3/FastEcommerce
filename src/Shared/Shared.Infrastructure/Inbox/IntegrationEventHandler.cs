using Shared.Contracts;

namespace Shared.Infrastructure.Inbox;

public abstract class IntegrationEventHandler<TEvent> : IIntegrationEventHandler<TEvent>
    where TEvent : IntegrationEvent
{
    public abstract Task Handle(TEvent integrationEvent, CancellationToken cancellationToken = default);

    public Task Handle(IntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
    {
        if (integrationEvent is not TEvent specificIntegrationEvent)
        {
            throw new ArgumentException($"Invalid integration event type: {integrationEvent.GetType().Name}");
        }

        return Handle(specificIntegrationEvent, cancellationToken);
    }
}
