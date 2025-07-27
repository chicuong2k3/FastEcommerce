using IdentityServer8;
using IdentityServer8.Models;

namespace IdentityServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            [
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            ];

        public static IEnumerable<ApiScope> ApiScopes =>
            [
                new ApiScope("read", "Read Permission"),
                new ApiScope("write", "Write Permission")
            ];

        public static IEnumerable<Client> Clients =>
            [
                new Client()
                {
                    ClientId = "fast-ecommerce-blazor",
                    AllowedGrantTypes = GrantTypes.Code,
                    ClientSecrets = [new ("secret".Sha256())],
                    RedirectUris = { "https://localhost:10001/signin-oidc" },
                    PostLogoutRedirectUris = { "https://localhost:10001/signout-callback-oidc" },
                    AllowedScopes =
                    [
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "read",
                        "write"
                    ]
                }
            ];
    }
}
