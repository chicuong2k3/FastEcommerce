using IdentityServer8.Models;

namespace IdentityServer.Extensions;

public static class AuthorizationRequestExtensions
{
    public static bool IsNativeClient(this AuthorizationRequest context)
    {
        return !context.RedirectUri.StartsWith("https", StringComparison.Ordinal)
           && !context.RedirectUri.StartsWith("http", StringComparison.Ordinal);
    }
}
