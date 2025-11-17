using ErrorOr;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Taskly.Api.Middleware;

public sealed class GlobalExceptionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext ctx)
    {
        try
        {
            await next(ctx);
        }
        catch (ValidationException validationEx)
        {
            ctx.Response.ContentType = "application/json";
            ctx.Response.StatusCode = StatusCodes.Status400BadRequest;

            var validationErrors = validationEx.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToList()
                );

            var error = Error.Validation(
                "Validation.Failed",
                "One or more validation errors occurred."
            );

            var problem = BuildValidationProblem(error, validationErrors);

            await ctx.Response.WriteAsJsonAsync(problem);
        }
        catch (Exception ex)
        {
            ctx.Response.ContentType = "application/json";
            ctx.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var error = Error.Unexpected("Unhandled.Exception", "An unhandled exception occurred.");

            var problem = BuildProblem(error);

            await ctx.Response.WriteAsJsonAsync(problem);
        }
    }

    private static ProblemDetails BuildValidationProblem(Error error, Dictionary<string, List<string>> validationErrors)
    {
        return new ProblemDetails
        {
            Title = error.Code,
            Detail = error.Description,
            Status = StatusCodes.Status400BadRequest,
            Type = "https://httpstatuses.com/400",
            Extensions = { ["errors"] = validationErrors }
        };
    }

    private static ProblemDetails BuildProblem(Error error)
    {
        return new ProblemDetails
        {
            Title = error.Code,
            Detail = error.Description,
            Status = StatusCodes.Status500InternalServerError,
            Type = "https://httpstatuses.com/500"
        };
    }
}