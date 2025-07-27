using IdentityServer.Data;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Services;

public class EfLoginTokenStore : ILoginTokenStore
{
    private readonly ApplicationDbContext _context;

    public EfLoginTokenStore(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<string> GenerateTokenAsync(string userId)
    {
        var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        var expiresAt = DateTime.UtcNow.AddSeconds(60);

        _context.LoginTokens.Add(new LoginToken
        {
            Id = Guid.NewGuid(),
            Token = token,
            UserId = userId,
            ExpiresAt = expiresAt
        });

        await _context.SaveChangesAsync();
        return token;
    }

    public async Task InvalidateTokenAsync(string userId)
    {
        var tokenEntries = await _context.LoginTokens
                                .Where(t => t.UserId == userId && t.ExpiresAt <= DateTime.UtcNow)
                                .ToListAsync();

        _context.LoginTokens.RemoveRange(tokenEntries);
        await _context.SaveChangesAsync();
    }

    public async Task<string?> ValidateTokenAsync(string token)
    {
        var tokenEntry = await _context.LoginTokens
            .FirstOrDefaultAsync(t => t.Token == token && t.ExpiresAt > DateTime.UtcNow);

        return tokenEntry?.UserId;
    }

}
