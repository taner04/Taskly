using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace Api.Composition.OpenApiDocumentTransformers;

public class BearerDocumentTransformer(IConfiguration configuration) : IOpenApiDocumentTransformer
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
                Description = "Enter 'Bearer' followed by a space and then the JWT in the requestheader."
            }
        };

        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes = securityRequirements;
        
        return Task.CompletedTask;
    }
}