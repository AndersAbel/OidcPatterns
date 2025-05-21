using Duende.IdentityServer;
using Serilog;

namespace IdSrv;

internal static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddRazorPages();

        var isBuilder = builder.Services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
            })
            .AddTestUsers(TestUsers.Users)
            .AddLicenseSummary();

        // in-memory, code config
        isBuilder.AddInMemoryIdentityResources(Config.IdentityResources);
        isBuilder.AddInMemoryClients(Config.Clients);


        builder.Services.AddAuthentication()
            .AddGoogle(options =>
            {
                options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                options.ClientId = "999560008429-i4fnstq91ek0sl61pqaics7toir0php5.apps.googleusercontent.com";
                options.ClientSecret = "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";

            })
            .AddOpenIdConnect("IdSrv", "Duende IdentityServer", opt =>
            {
                opt.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                opt.SignOutScheme = IdentityServerConstants.SignoutScheme;

                opt.Authority = "https://demo.duendesoftware.com";

                opt.ClientId = "interactive.confidential";
                opt.ClientSecret = "secret";

                opt.ResponseType = "code";

                opt.GetClaimsFromUserInfoEndpoint = true;

                opt.CallbackPath = "/signin-idsrv";
                opt.SignedOutCallbackPath = "/signout-callback-idsrv";

                opt.SaveTokens = true;
            })
            .AddOpenIdConnect("Entra", "Entra Id Common", opt =>
            {
                opt.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                opt.SignOutScheme = IdentityServerConstants.SignoutScheme;

                opt.Authority = "https://login.microsoftonline.com/common";

                opt.ClientId = "3d5fb20a-3cff-48d5-8b22-5d207051d916";
                opt.ClientSecret = "19o8Q~d8Fdpobh6bwtvnBoWTmveME1.W7-bUBbYm"; // Expires 2027-05-21

                opt.CallbackPath = "/signin-entra-common";
                opt.SignedOutCallbackPath = "/signout-callback-entra-common";

                opt.TokenValidationParameters.ValidateIssuer = false;

                opt.ResponseType = "code";
            });

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.UseSerilogRequestLogging();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseStaticFiles();
        app.UseRouting();
        app.UseIdentityServer();
        app.UseAuthorization();

        app.MapRazorPages()
            .RequireAuthorization();

        return app;
    }
}
