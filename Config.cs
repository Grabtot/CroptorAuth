using Duende.IdentityServer.Models;

namespace CroptorAuth
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResource("plan", ["plan"])
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("croptor.read"),
                new ApiScope("croptor.write"),
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                new Client
                {
                    ClientId = "interactive",
                    ClientSecrets = { new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256()) },

                    AllowedGrantTypes = GrantTypes.Code,

                    RedirectUris = { "http://localhost:3000/signin-oidc" },
                    FrontChannelLogoutUri = "http://localhost:3000/signout-oidc",
                    PostLogoutRedirectUris = { "http://localhost:3000/signout-callback-oidc" },

                    AllowedCorsOrigins = { "http://localhost:3000" },
                    AllowOfflineAccess = true,

                    AllowedScopes = { "openid", "profile", "croptor.read", "croptor.write","plan" },

                }
            };
    }
}
