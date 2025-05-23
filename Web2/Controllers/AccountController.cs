﻿using Duende.IdentityModel;
using Duende.IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace Web2.Controllers;

public class AccountController
    (LogoutSessions logoutSessions) : Controller
{
    [HttpPost]
    public IActionResult Logout()
    {
        AuthenticationProperties props = new()
        {
            RedirectUri = "/"
        };

        return SignOut(props,
            OpenIdConnectDefaults.AuthenticationScheme,
            CookieAuthenticationDefaults.AuthenticationScheme);
    }

    [HttpPost]
    [AllowAnonymous]
    [IgnoreAntiforgeryToken]

    public async Task<IActionResult> BackChannelLogout(string logout_token)
    {
        Response.Headers.Append("Cache-Control", "no-cache, no-store");
        Response.Headers.Append("Pragma", "no-cache");

        try
        {
            var user = await ValidateLogoutToken(logout_token);

            // these are the sub & sid to signout
            var sub = user.FindFirst("sub")?.Value;
            var sid = user.FindFirst("sid")?.Value;

            logoutSessions.Add(sub!, sid!);

            return Ok();
        }
        catch { }

        return BadRequest();
    }
    private async Task<ClaimsPrincipal> ValidateLogoutToken(string logoutToken)
    {
        var claims = await ValidateJwt(logoutToken);

        if (claims.FindFirst("sub") == null && claims.FindFirst("sid") == null) throw new Exception("Invalid logout token");

        var nonce = claims.FindFirstValue("nonce");
        if (!string.IsNullOrWhiteSpace(nonce)) throw new Exception("Invalid logout token");

        var eventsJson = claims.FindFirst("events")?.Value;
        if (string.IsNullOrWhiteSpace(eventsJson)) throw new Exception("Invalid logout token");

        var events = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(eventsJson);
        var logoutEvent = events!.TryGetValue("http://schemas.openid.net/event/backchannel-logout", out _);
        if (logoutEvent == false) throw new Exception("Invalid logout token");

        return claims;
    }

    private static async Task<ClaimsPrincipal> ValidateJwt(string jwt)
    {
        // read discovery document to find issuer and key material
        var client = new HttpClient();
        var disco = await client.GetDiscoveryDocumentAsync("https://localhost:5000");

        var keys = new List<SecurityKey>();
        foreach (var webKey in disco.KeySet!.Keys)
        {
            var key = new JsonWebKey()
            {
                Kty = webKey.Kty,
                Alg = webKey.Alg,
                Kid = webKey.Kid,
                X = webKey.X,
                Y = webKey.Y,
                Crv = webKey.Crv,
                E = webKey.E,
                N = webKey.N,
            };
            keys.Add(key);
        }

        var parameters = new TokenValidationParameters
        {
            ValidIssuer = disco.Issuer,
            ValidAudience = "Web2",
            IssuerSigningKeys = keys,

            NameClaimType = JwtClaimTypes.Name,
            RoleClaimType = JwtClaimTypes.Role
        };

        var handler = new JwtSecurityTokenHandler();
        handler.InboundClaimTypeMap.Clear();

        var user = handler.ValidateToken(jwt, parameters, out var _);
        return user;
    }
}
