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
        });

        return app;
    }
}