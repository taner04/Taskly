using Api.Composition.OpenApiDocumentTransformers;
using Microsoft.AspNetCore.OpenApi;

namespace Api.Composition.Configs;

public static class OpenApiConfig
{
    public static void Config(
        OpenApiOptions options)
    {
        options.AddDocumentTransformer<BearerDocumentTransformer>();
    }
}