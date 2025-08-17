namespace IdentityServer.Services;

public interface IEmailService
{
    Task<bool> SendEmailAsync(string toName, string toEmail, string subject, string? content);
}