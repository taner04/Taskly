using Api.Composition.Options;
using Scalar.AspNetCore;

namespace Api.Composition.ServiceExtensions;

internal static class ScalarExtension
{
    extension(WebApplication app)
    {
        internal WebApplication MapScalar()
        {
            var auth0Options = app.Configuration.GetSection("Auth0").Get<Auth0Options>() ??
                               throw new InvalidOperationException("Auth0 configuration is missing.");

            app.MapScalarApiReference(opt =>
            {
                opt.Layout = ScalarLayout.Classic;
                opt.Theme = ScalarTheme.DeepSpace;
                opt.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.RestSharp);
                opt.AddPreferredSecuritySchemes("OAuth2")
                    .AddOAuth2Authentication("OAuth2", scheme => scheme
                        .WithFlows(flows => flows
                            .WithAuthorizationCode(flow => flow
                                .WithAuthorizationUrl($"https://{auth0Options.Domain}/authorize")
                                .WithTokenUrl($"https://{auth0Options.Domain}/oauth/token")
                                .WithClientId(auth0Options.ClientId)
                                .WithClientSecret(auth0Options.ClientSecret)
                                .WithPkce(Pkce.Sha256)
                                .AddQueryParameter("audience", auth0Options.Audience)
                            ))
                        .WithDefaultScopes("openid", "profile", "api")
                    );

                if (auth0Options.UsePersistentStorage)
                {
                    opt.EnablePersistentAuthentication();
                }
            });

            return app;
        }   
    }
}