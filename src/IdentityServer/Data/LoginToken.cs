namespace IdentityServer.Data;

public class LoginToken
{
    public Guid Id { get; set; }
    public string Token { get; set; } = default!;
    public string UserId { get; set; } = default!;
    public DateTime ExpiresAt { get; set; }
}
