using Microsoft.AspNetCore.OpenApi;

namespace Taskly.WebApi.Common.Composition.Configs.OpenApi;

internal static class OpenApiConfig
{
    internal static void Config(
        OpenApiOptions options)
    {
        options.AddDocumentTransformer<BearerDocumentTransformer>();
    }
}