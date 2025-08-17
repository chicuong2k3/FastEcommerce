using Radzen;

namespace Administration.Client.Services;

public class NotifService : INotifService
{
    private readonly NotificationService _notificationService;

    public NotifService(NotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public void Error(string message)
    {
        _notificationService.Notify(new NotificationMessage()
        {
            Severity = NotificationSeverity.Error,
            Summary = message,
            Duration = 2000,
            Style = "position: absolute; inset-inline-start: -350px; top: -100px",
            ShowProgress = true
        });
    }

    public void Success(string message)
    {
        _notificationService.Notify(new NotificationMessage()
        {
            Severity = NotificationSeverity.Success,
            Summary = message,
            Duration = 2000,
            Style = "position: absolute; inset-inline-start: -350px; top: -100px",
            ShowProgress = true
        });
    }
}
