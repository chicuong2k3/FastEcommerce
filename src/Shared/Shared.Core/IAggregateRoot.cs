namespace Shared.Core;

public interface IAggregateRoot
{
    void Raise(DomainEvent domainEvent);
    IReadOnlyCollection<DomainEvent> GetDomainEvents();
    void ClearDomainEvents();
}

public interface IAggregateRoot<T> : IAggregateRoot
{
    T Id { get; }
}