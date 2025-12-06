using Api.Composition.Options;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace Api.Composition.OpenApiDocumentTransformers;

internal sealed class BearerDocumentTransformer(IConfiguration configuration) : IOpenApiDocumentTransformer
{
    public Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        var auth0Options = configuration.GetSection("Auth0").Get<Auth0Options>() ??
                           throw new InvalidOperationException("Auth0 configuration is missing.");

        document.Components ??= new OpenApiComponents();

        // bestehendes JWT Bearer Scheme
        document.Components.SecuritySchemes["JWT"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "JWT Bearer Token"
        };

        // NEU: OAuth2 Authorization Code Flow für Scalar Auth
        document.Components.SecuritySchemes["OAuth2"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.OAuth2,
            Description = "Auth0 OAuth2 Login",
            Flows = new OpenApiOAuthFlows
            {
                AuthorizationCode = new OpenApiOAuthFlow
                {
                    AuthorizationUrl = new Uri($"https://{auth0Options.Domain}/authorize"),
                    TokenUrl = new Uri($"https://{auth0Options.Domain}/oauth/token"),
                    Scopes = new Dictionary<string, string>()
                }
            }
        };

        return Task.CompletedTask;
    }
}