namespace IdentityServer.Services;

public interface ILoginTokenStore
{
    Task<string> GenerateTokenAsync(string userId);
    Task<string?> ValidateTokenAsync(string token);
    Task InvalidateTokenAsync(string userId);
}
