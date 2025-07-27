using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Core;

public abstract class AggregateRoot<T> : Entity<T>, IAggregateRoot<T>
{
    [NotMapped]
    private readonly List<DomainEvent> domainEvents = [];

    public IReadOnlyCollection<DomainEvent> GetDomainEvents()
    {
        return [.. domainEvents];
    }

    public void ClearDomainEvents()
    {
        domainEvents.Clear();
    }

    public void Raise(DomainEvent domainEvent)
    {
        domainEvents.Add(domainEvent);
    }
}
