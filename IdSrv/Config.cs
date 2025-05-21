using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace IdSrv;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        [
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
        ];

    public static IEnumerable<Client> Clients =>
        [
            new()
            {
                ClientId = "Web1",

                RedirectUris = { "https://localhost:5001/signin-oidc" },

                AllowedGrantTypes = GrantTypes.Implicit,

                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile
                },

                PostLogoutRedirectUris =
                {
                    "https://localhost:5001"
                },

                FrontChannelLogoutUri = "https://localhost:5001/Account/Logout"
            },
            new()
            {
                ClientId = "Web2",
                ClientSecrets = { new("Secret2".Sha256() )},

                RedirectUris = { "https://localhost:5002/signin-oidc" },

                AllowedGrantTypes = GrantTypes.Code,

                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile
                },

                PostLogoutRedirectUris =
                {
                    "https://localhost:5002/signout-callback-oidc"
                },

                BackChannelLogoutUri = "https://localhost:5002/Account/BackChannelLogout"
            }
        ];
}
