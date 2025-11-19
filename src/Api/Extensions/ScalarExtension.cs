using Scalar.AspNetCore;

namespace Api.Extensions;

public static class ScalarExtension
{
    public static WebApplication MapScalar(this WebApplication app)
    {
        app.MapScalarApiReference(opt =>
        {
            opt.Layout = ScalarLayout.Classic;
            opt.Theme = ScalarTheme.DeepSpace;
        });

        return app;
    }
}