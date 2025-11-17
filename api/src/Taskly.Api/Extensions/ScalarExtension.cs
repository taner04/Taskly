using Scalar.AspNetCore;

namespace Taskly.Api.Extensions;

public static class ScalarExtension
{
    extension(WebApplication app)
    {
        public WebApplication MapScalar()
        {
            app.MapScalarApiReference(opt => { opt.Layout = ScalarLayout.Classic; });

            return app;
        }
    }
}