using Scalar.AspNetCore;

namespace Api.Composition.ServiceExtensions;

internal static class ScalarExtension
{
    internal static WebApplication MapScalar(
        this WebApplication app)
    {
        app.MapScalarApiReference(opt =>
        {
            opt.Layout = ScalarLayout.Modern;
            opt.Theme = ScalarTheme.DeepSpace;
            opt.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.RestSharp);
        });

        return app;
    }
}