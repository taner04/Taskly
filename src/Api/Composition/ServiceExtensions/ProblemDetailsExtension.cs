using System.Diagnostics;
using Api.Features.Shared.Api;

namespace Api.Composition.ServiceExtensions;

internal static class ProblemDetailsExtension
{
    internal static IServiceCollection AddCustomizedProblemDetails(
        this IServiceCollection services)
    {
        services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = ctx =>
            {
                var httpContext = ctx.HttpContext;

                var problemDetails = ctx.Exception switch
                {
                    ValidationException validation => new ApiProblemDetails
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Title = "Validation failed.",
                        ErrorCode = "validation.error",
                        Errors = validation.Errors
                            .GroupBy(e => e.PropertyName, StringComparer.OrdinalIgnoreCase)
                            .ToDictionary(
                                g => g.Key,
                                g => g.Select(e => e.ErrorMessage).ToArray(),
                                StringComparer.OrdinalIgnoreCase
                            )
                    },
                    
                    UnauthorizedAccessException => new ApiProblemDetails
                    {
                        Status = StatusCodes.Status401Unauthorized,
                        Title = "Unauthorized",
                        Detail = "You are not authorized to access this resource.",
                        ErrorCode = "Unauthorized.Access"
                    },

                    _ => new ApiProblemDetails
                    {
                        Status = StatusCodes.Status500InternalServerError,
                        Title = "Internal Server Error",
                        Detail = "An unexpected error has occurred.",
                        ErrorCode = "server.error"
                    }
                };
                
                httpContext.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
                
                problemDetails.Instance = httpContext.Request.Path;
                problemDetails.Type = $"https://taskly.com/{GetRoutePattern(httpContext)}";
                
                problemDetails.Extensions["method"] = httpContext.Request.Method;
                problemDetails.Extensions["traceId"] = Activity.Current?.Id ?? httpContext.TraceIdentifier;
                problemDetails.Extensions["errorCode"] = problemDetails.ErrorCode;

                if (problemDetails.Errors.Count > 0)
                {
                    problemDetails.Extensions["errors"] = problemDetails.Errors;
                }

                ctx.ProblemDetails = problemDetails;
            };
        });

        return services;
    }
    
    private static string GetRoutePattern(HttpContext http)
    {
        var endpoint = http.GetEndpoint();
        return endpoint?
            .Metadata
            .GetMetadata<RouteEndpoint>()?
            .RoutePattern.RawText
            ?? http.Request.Path;
    }
}
