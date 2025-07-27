namespace NotificationService.Core;

public interface IEmailSender
{
    Task SendEmailAsync(string from, string to, string subject, string? title, object content, CancellationToken cancellationToken = default);
}
