using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.Security.Claims;
using Web2;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(opt =>
{
    opt.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
    .AddCookie(opt =>
    {
        opt.Cookie.Name = "Web2";

        opt.Events.OnValidatePrincipal = async ctx =>
        {
            var sid = ctx.Principal!.FindFirstValue("sid");
            var sub = ctx.Principal!.FindFirstValue("sub");

            var logoutSessions = ctx.HttpContext.RequestServices.GetRequiredService<LogoutSessions>();

            if (logoutSessions.IsLoggedOut(sub!, sid!))
            {
                ctx.RejectPrincipal();
                await ctx.HttpContext.SignOutAsync();
            }
        };
    })
    .AddOpenIdConnect(opt =>
    {
        opt.Authority = "https://localhost:5000";
        opt.ClientId = "Web2";
        opt.ClientSecret = "Secret2";

        opt.ResponseType = "code";

        opt.SaveTokens = true;

        opt.MapInboundClaims = false;
    });

builder.Services.AddSingleton<LogoutSessions>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
