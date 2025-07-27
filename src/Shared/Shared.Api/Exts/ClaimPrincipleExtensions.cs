using System.Security.Claims;

namespace Shared.Api.Exts;

public static class ClaimPrincipleExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal claimsPrincipal)
    {
        return new Guid("f208fd56-bb1e-4a1b-8ae7-9290e3368e23");
    }
}
