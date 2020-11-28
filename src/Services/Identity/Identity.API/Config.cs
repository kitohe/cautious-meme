using System.Collections.Generic;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;

namespace Identity.API
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> Ids =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };

        public static IEnumerable<ApiScope> Apis =>
            new List<ApiScope>
            {
                new("messaging.api", "Messaging API"),
            };

        public static IEnumerable<Client> Clients =>
            new List<Client>
            {
                // interactive ASP.NET Core MVC client
                new()
                {
                    ClientId = "cmessaging",
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.Code,
                    RequireConsent = false,

                    // where to redirect to after login
                    RedirectUris = { "http://localhost:5500/signin-oidc" },

                    // where to redirect to after logout
                    PostLogoutRedirectUris = { "http://localhost:5500/signout-callback-oidc" },

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "messaging.api"
                    },
                    AllowOfflineAccess = true,

                    Claims = new List<ClientClaim>
                    {
                        new(JwtClaimTypes.Role, "Admin"),
                        new(JwtClaimTypes.Role, "User"),
                    },
                    AlwaysSendClientClaims = true,
                    RequirePkce = true
                }
            };
    }
}
