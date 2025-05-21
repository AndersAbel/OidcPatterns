using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Web1.Controllers;

public class AccountController : Controller
{
    [HttpPost]

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();

        return Redirect("/");
    }
}
