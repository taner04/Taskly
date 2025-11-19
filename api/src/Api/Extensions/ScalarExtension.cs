using Scalar.AspNetCore;

namespace Api.Extensions;

public static class ScalarExtension
{
    extension(WebApplication app)
    {
        public WebApplication MapScalar()
        {
            app.MapScalarApiReference(opt =>
            {
                opt.Layout = ScalarLayout.Classic;
                opt.Theme = ScalarTheme.DeepSpace;
            });

            return app;
        }
    }
}