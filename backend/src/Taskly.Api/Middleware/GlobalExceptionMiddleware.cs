using FluentValidation;
using Taskly.Core.Functional;

namespace Taskly.Api.Middleware;

public sealed class GlobalExceptionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext ctx)
    {
        try
        {
            await next(ctx);
        }
        catch (Exception ex)
        {
            ctx.Response.ContentType = "application/json";
            
            if(ex is ValidationException validationEx)
            {
                var validationErrors = validationEx.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToList()
                    );

                var validationError = Error.Validation("Validation failed", "One or more validation error occured.", validationErrors);
                
                ctx.Response.StatusCode = 400;
                
                await ctx.Response.WriteAsJsonAsync(validationError);
                return;
            }
            
            var error = Error.Internal("Unhandled Exception", "An unhandled exception occurred.");
            
            ctx.Response.StatusCode = 500;

            await ctx.Response.WriteAsJsonAsync(error);
        }
    }
}