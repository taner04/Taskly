using Hangfire;
using Scalar.AspNetCore;
using Taskly.Shared.Extensions;
using Taskly.WebApi.Common.Composition.Options;
using AttachmentBlobContainerService =
    Taskly.WebApi.Features.Attachments.Common.Services.AttachmentBlobContainerService;

namespace Taskly.WebApi.Common.Composition.Extensions;

internal static class WebApplicationExtensions
{
    extension(WebApplication app)
    {
        internal WebApplication MapScalar()
        {
            var auth0Config = app.Configuration.GetOptions<Auth0Config>();

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

        internal async Task<WebApplication> InitializeBlobStorage()
        {
            using var scope = app.Services.CreateScope();
            var attachmentService = scope.ServiceProvider.GetRequiredService<AttachmentBlobContainerService>();

            await attachmentService.InitializeAsync();

            return app;
        }

        internal WebApplication AddHangfireDashboard()
        {
            app.UseHangfireDashboard(options: new DashboardOptions
            {
                DarkModeEnabled = true
            });

            return app;
        }
    }
}