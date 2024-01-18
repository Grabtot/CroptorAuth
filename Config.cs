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
                new ApiScope("croptor")
                {
                    UserClaims = ["plan"]
                },
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                new Client
                {
                    ClientId = "web",
                    ClientSecrets = { new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256()) },

                    AllowedGrantTypes = GrantTypes.Code,

                    RedirectUris = [ "http://localhost:3000/signin-oidc", "https://croptor.com/signin-oidc" ],
                    FrontChannelLogoutUri = "http://localhost:3000/signout-oidc",
                    PostLogoutRedirectUris = { "http://localhost:3000/signout-callback-oidc",
                        "https://croptor.com//signout-callback-oidc" },

                    AllowedCorsOrigins = { "http://localhost:3000" ,"https://croptor.com"},
                    AllowOfflineAccess = true,

                    AllowedScopes = { "openid", "profile", "croptor" },

                }
            };
    }
}
