using System.Diagnostics.CodeAnalysis;
using Auth0.AspNetCore.Authentication;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Authentication;

namespace Web.Features.Authenitcation;

[Handler]
[MapGet("/account/login")]
public static partial class Login
{
    [SuppressMessage("ImmediateHandler", "IHR0012:Handler method should use CancellationToken",
        Justification = "<Outgoing>")]
    private static async ValueTask HandleAsync(Query _, IHttpContextAccessor accessor)
    {
        var authenticationProperties = new LoginAuthenticationPropertiesBuilder()
            .WithRedirectUri("/")
            .Build();

        await accessor.HttpContext!.ChallengeAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
    }

    // ReSharper disable once ClassNeverInstantiated.Global
    public sealed record Query;
}