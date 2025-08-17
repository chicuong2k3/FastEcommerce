
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace IdentityServer.Services;

public class EmailOptions
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
    public string FromName { get; set; } = string.Empty;
    public string FromEmail { get; set; } = string.Empty;
}

public class EmailService : IEmailService
{
    private readonly EmailOptions _options;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<EmailOptions> options, ILogger<EmailService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task<bool> SendEmailAsync(string toName, string toEmail, string subject, string? content)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(_options.FromName, _options.FromEmail));
        email.To.Add(new MailboxAddress(toName, toEmail));

        email.Subject = subject;
        email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
        {
            Text = content
        };

        try
        {
            using (var smtp = new SmtpClient())
            {

                await smtp.ConnectAsync(_options.Host, _options.Port, SecureSocketOptions.StartTls);

                await smtp.AuthenticateAsync(_options.Username, _options.Password);

                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                return true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Error: {Error}", ex.Message);
            return false;
        }
    }
}
