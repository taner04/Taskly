using ErrorOr;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Taskly.Api.ExceptionHandlers;

// Modern .NET Exception Handler (ASP.NET Core 7+)
public sealed class GlobalExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        httpContext.Response.ContentType = "application/json";

        // ----------------------------------------------------
        // VALIDATION EXCEPTION
        // ----------------------------------------------------
        if (exception is ValidationException validationEx)
        {
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

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

            await problemDetailsService.WriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails = problem,
                Exception = exception
            });

            return true;
        }

        // ----------------------------------------------------
        // UNHANDLED / UNKNOWN EXCEPTION
        // ----------------------------------------------------
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var unexpected = Error.Unexpected(
            "Unhandled.Exception",
            "An unhandled exception occurred."
        );

        var unexpectedProblem = BuildProblem(unexpected);

        await problemDetailsService.WriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = unexpectedProblem,
            Exception = exception
        });

        return true;
    }

    private static ProblemDetails BuildValidationProblem(
        Error error,
        Dictionary<string, List<string>> validationErrors)
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
