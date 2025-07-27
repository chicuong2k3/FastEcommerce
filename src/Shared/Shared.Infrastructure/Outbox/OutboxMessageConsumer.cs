using System.ComponentModel.DataAnnotations;

namespace Shared.Infrastructure.Outbox;

public class OutboxMessageConsumer
{
    [Key]
    public Guid Id { get; private set; }
    public Guid OutboxMessageId { get; private set; }
    [MaxLength(250)]
    public string Name { get; private set; }

    private OutboxMessageConsumer() { } // For EF Core

    public OutboxMessageConsumer(Guid outboxMessageId, string name)
    {
        Id = Guid.NewGuid();
        OutboxMessageId = outboxMessageId;
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

}
