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
        ];
}
