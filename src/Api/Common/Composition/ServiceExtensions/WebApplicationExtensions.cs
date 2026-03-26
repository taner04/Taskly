using Api.Common.Composition.Options;
using Scalar.AspNetCore;

namespace Api.Common.Composition.ServiceExtensions;

public static class WebApplicationExtensions
{
    internal static WebApplication MapScalar(this WebApplication app)
    {
        var auth0Config = app.Configuration.GetSection(nameof(Auth0Config)).Get<Auth0Config>();
        ArgumentNullException.ThrowIfNull(auth0Config);

            app.MapScalarApiReference(opt =>
            {
                opt.Layout = ScalarLayout.Classic;
                opt.Theme = ScalarTheme.DeepSpace;
                opt.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.RestSharp);
                opt.AddPreferredSecuritySchemes("OAuth2")
                    .AddOAuth2Authentication("OAuth2", scheme => scheme
                        .WithFlows(flows => flows
                            .WithAuthorizationCode(flow => flow
                                .WithAuthorizationUrl($"https://{auth0Config.Domain}/authorize")
                                .WithTokenUrl($"https://{auth0Config.Domain}/oauth/token")
                                .WithClientId(auth0Config.ClientId)
                                .WithClientSecret(auth0Config.ClientSecret)
                                .WithPkce(Pkce.Sha256)
                                .AddQueryParameter("audience", auth0Config.Audience)
                            ))
                        .WithDefaultScopes("openid", "profile", "email", "email_verified")
                    );

                if (auth0Config.UsePersistentStorage)
                {
                    opt.EnablePersistentAuthentication();
                }
            });

            return app;
    }
}