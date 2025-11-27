using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace Api.Composition.OpenApiDocumentTransformers;

internal sealed class BearerDocumentTransformer(IConfiguration configuration) : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        var securityRequirements = new Dictionary<string, OpenApiSecurityScheme>
        {
            ["JWT"] = new()
            {
                In = ParameterLocation.Header,
                Name = "Bearer",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                Description = "Enter 'Bearer' followed by a space and the JWT in the request header."
            }
        };

        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes = securityRequirements;
        
        return Task.CompletedTask;
    }
}