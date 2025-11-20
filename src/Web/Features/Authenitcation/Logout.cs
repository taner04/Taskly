using System.Diagnostics.CodeAnalysis;
using Auth0.AspNetCore.Authentication;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Web.Features.Authenitcation;

[Handler]
[MapGet("/account/logout")]
public static partial class Logout
{
    [SuppressMessage("ImmediateHandler", "IHR0012:Handler method should use CancellationToken",
        Justification = "<Outgoing>")]
    private static async ValueTask HandleAsync(Query _, IHttpContextAccessor accessor)
    {
        var authenticationProperties = new LogoutAuthenticationPropertiesBuilder()
            .WithRedirectUri("/")
            .Build();

        await accessor.HttpContext!.SignOutAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
        await accessor.HttpContext!.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }

    // ReSharper disable once ClassNeverInstantiated.Global
    public sealed record Query;
}