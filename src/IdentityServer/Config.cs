using IdentityServer8;
using IdentityServer8.Models;

namespace IdentityServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            [
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResource()
                {
                    Name = "roles",
                    DisplayName = "User role(s)",
                    UserClaims = ["role"]
                }
            ];

        public static IEnumerable<ApiScope> ApiScopes =>
            [
                new ApiScope("product.read"),
                new ApiScope("product.write"),
                new ApiScope("category.read"),
                new ApiScope("category.write"),

                new ApiScope("cart.read"),
                new ApiScope("cart.write"),
                new ApiScope("order.read"),
                new ApiScope("order.write"),
            ];

        public static IEnumerable<ApiResource> ApiResources =>
            [
                new ApiResource("catalog-api", "Catalog API")
                {
                    Scopes = [
                        "product.read",
                        "product.write",
                        "category.read",
                        "category.write"
                    ]
                },
                new ApiResource("ordering-api", "Ordering API")
                {
                    Scopes = [
                        "cart.read",
                        "cart.write",
                        "order.read",
                        "order.write"
                    ]
                }
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
                        "roles",
                        "product.read", "category.read",
                        "cart.read", "cart.write",
                        "order.read"
                    ],
                    AllowOfflineAccess = true,
                    RequirePkce = true
                }
            ];
    }
}
