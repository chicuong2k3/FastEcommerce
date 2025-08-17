using Shared.Core;

namespace NotificationService.Core;

public class Notification : AggregateRoot<Guid>
{
    public string From { get; set; }
    public string To { get; set; }
    public string Message { get; set; }
    public DateTime SentAt { get; set; }
    public bool IsSuccess { get; set; }
}
