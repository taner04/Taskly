using Scalar.AspNetCore;

namespace Api.Composition.ServiceExtensions;

internal static class ScalarExtension
{
    internal static WebApplication MapScalar(
        this WebApplication app)
    {
        var auth0Domain  = app.Configuration["Auth0:Domain"];   // z.B. "meine-domain.eu.auth0.com"
        var auth0ClientId = app.Configuration["Auth0:ClientId"];
        var auth0Audience = app.Configuration["Auth0:Audience"]; // deine API-Audience
        
        app.MapScalarApiReference(opt =>
        {
            opt.Layout = ScalarLayout.Classic;
            opt.Theme = ScalarTheme.DeepSpace;
            opt.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.RestSharp);
            opt.AddPreferredSecuritySchemes("OAuth2") // Name des securitySchemes aus dem OpenAPI
                .AddOAuth2Authentication("OAuth2", scheme => scheme
                    .WithFlows(flows => flows
                        .WithAuthorizationCode(flow => flow
                            .WithAuthorizationUrl($"https://{auth0Domain}/authorize")
                            .WithTokenUrl($"https://{auth0Domain}/oauth/token")
                            .WithClientId(auth0ClientId)
                            .WithPkce(Pkce.Sha256)
                            .WithSelectedScopes("openid", "profile", "api.read")
                            .AddQueryParameter("audience", auth0Audience)
                        ))
                    .WithDefaultScopes("openid", "profile", "api.read")
                ).EnablePersistentAuthentication();
        });

        return app;
    }
}