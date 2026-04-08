using Taskly.WebApi.Common.Composition.Options;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using Taskly.Shared.Extensions;

namespace Taskly.WebApi.Common.Composition.Configs.OpenApi;

internal sealed class BearerDocumentTransformer(IConfiguration configuration) : IOpenApiDocumentTransformer
{
    public Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        document.Components ??= new OpenApiComponents();

        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

        document.Components.SecuritySchemes["JWT"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "JWT Bearer Token"
        };

        var auth0Config = configuration.GetOptions<Auth0Config>();

        document.Components.SecuritySchemes["OAuth2"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.OAuth2,
            Description = "Auth0 OAuth2 Login",
            Flows = new OpenApiOAuthFlows
            {
                AuthorizationCode = new OpenApiOAuthFlow
                {
                    AuthorizationUrl = new Uri($"https://{auth0Config.Domain}/authorize"),
                    TokenUrl = new Uri($"https://{auth0Config.Domain}/oauth/token"),
                    Scopes = new Dictionary<string, string>()
                }
            }
        };

        return Task.CompletedTask;
    }
}
