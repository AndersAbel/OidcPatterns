using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web2.Models;

namespace Web2.Controllers;

[Authorize]
public class SecureController : Controller
{
    public async Task<IActionResult> Index()
    {
        var authResult = await HttpContext.AuthenticateAsync();

        return View(new SecureModel
        {
            Claims = authResult.Principal!.Claims,
            Properties = authResult.Properties!.Items!
        });
    }
}
