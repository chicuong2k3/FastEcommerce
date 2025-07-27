namespace Shared.Infrastructure.Inbox;

public class InboxOptions
{
    public int MessagesPerPoll { get; set; }
    public int PollIntervalInSeconds { get; set; }
}
