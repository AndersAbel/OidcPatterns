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

        //return Redirect("/");

        var idToken = await HttpContext.GetTokenAsync("id_token");

        return Redirect("https://localhost:5000/connect/endsession?" +
            $"id_token_hint={idToken}&" +
            "post_logout_redirect_uri=https://localhost:5001");
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
