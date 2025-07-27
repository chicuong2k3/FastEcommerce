using Microsoft.Extensions.Options;
using NotificationService.Core;

namespace NotificationService.Infrastructure;

internal class EmailSender : IEmailSender
{
    private readonly EmailSettings _emailSettings;

    public EmailSender(IOptions<EmailSettings> emailSettings)
    {
        _emailSettings = emailSettings.Value;
    }
    public Task SendEmailAsync(string from, string to, string subject, string? title, object content, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
