using Scalar.AspNetCore;

namespace Api.Composition.ServiceExtensions;

public static class ScalarExtension
{
    public static WebApplication MapScalar(this WebApplication app)
    {
        app.MapScalarApiReference(opt =>
        {
            opt.Layout = ScalarLayout.Classic;
            opt.Theme = ScalarTheme.DeepSpace;
            opt.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.RestSharp);
            opt.AddAuthorizationCodeFlow("OAuth2", flow =>
            {
                flow.ClientId = "mpzj3uPFqhoRqWpKMXiXdaAFUq7lf15I";
                flow.ClientSecret = "EKxln-L-zM50YyNsIZP0MyH9r3lHQOwbjF3rnwSN5a3wxYYtSltbGnQOVlAzT65K";
                flow.AuthorizationUrl = $"https://{app.Configuration["Auth0:Domain"]}/authorize";
                flow.RedirectUri = "http://localhost:5226/scalar/v1";
                
                flow.AddQueryParameter("audience", app.Configuration["Auth0:Audience"]!);
            });
        });

        return app;
    }
}