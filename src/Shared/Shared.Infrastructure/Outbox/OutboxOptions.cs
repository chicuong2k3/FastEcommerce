namespace Shared.Infrastructure.Outbox;

public class OutboxOptions
{
    public int MessagesPerPoll { get; set; }
    public int PollIntervalInSeconds { get; set; }

}
