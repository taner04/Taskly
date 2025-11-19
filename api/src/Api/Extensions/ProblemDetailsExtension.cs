using Microsoft.AspNetCore.Mvc;

namespace Api.Extensions;

public static class ProblemDetailsExtension
{
    public static IServiceCollection AddCustomizedProblemDetails(this IServiceCollection services)
    {
        services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = ctx =>
            {
                var http = ctx.HttpContext;

                if (ctx.Exception is null)
                {
                    AddDefaults(ctx.ProblemDetails, http);
                    return;
                }

                ctx.ProblemDetails = ctx.Exception switch
                {
                    ValidationException ex => new ValidationProblemDetails(
                        ex.Errors
                            .GroupBy(e => e.PropertyName, StringComparer.OrdinalIgnoreCase)
                            .ToDictionary(
                                g => g.Key,
                                g => g.Select(e => e.ErrorMessage).ToArray(),
                                StringComparer.OrdinalIgnoreCase
                            )
                    )
                    {
                        Status = StatusCodes.Status400BadRequest
                    },

                    _ => new ProblemDetails
                    {
                        Detail = "An error has occurred.",
                        Status = StatusCodes.Status500InternalServerError
                    }
                };

                AddDefaults(ctx.ProblemDetails, http);

                http.Response.StatusCode =
                    ctx.ProblemDetails.Status ?? StatusCodes.Status500InternalServerError;
            };
        });

        return services;
    }

    private static void AddDefaults(ProblemDetails problem, HttpContext http)
    {
        problem.Instance ??= http.Request.Path;
        problem.Extensions["traceId"] = http.TraceIdentifier;
        problem.Extensions["method"] = http.Request.Method;
    }
}