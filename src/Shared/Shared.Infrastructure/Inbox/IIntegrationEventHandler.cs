using Shared.Contracts;

namespace Shared.Infrastructure.Inbox;

public interface IIntegrationEventHandler<TEvent> : IIntegrationEventHandler
    where TEvent : IntegrationEvent
{
    Task Handle(TEvent @event, CancellationToken cancellationToken = default);
}

public interface IIntegrationEventHandler
{
    Task Handle(IntegrationEvent @event, CancellationToken cancellationToken = default);
}