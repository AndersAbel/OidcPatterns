using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Web1.Controllers;

public class AccountController : Controller
{
    [HttpPost]

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();

        // return Redirect("/");

        var idToken = await HttpContext.GetTokenAsync("id_token");

        var returnUrl = Uri.EscapeDataString("https://localhost:5001");

        return Redirect("https://localhost:5000/connect/endsession" +
            $"?post_logout_redirect_uri={returnUrl}&id_token_hint={idToken}");
    }

    [HttpGet]
    public async Task<IActionResult> Logout(string sid)
    {
        var currentSid = User.FindFirstValue("sid");
        if (currentSid == sid)
        {
            await HttpContext.SignOutAsync();
        }

        return Ok();
    }
}
