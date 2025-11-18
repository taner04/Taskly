using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using Taskly.Api.Abstractions;

namespace Taskly.Api.Features.Shared;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    /// <summary>
    /// For successful update/delete operations (PUT/DELETE)
    /// </summary>
    protected static IActionResult MapNoContentResult(ErrorOr<Success> result)
        => result.Match<IActionResult>(
            _ => new NoContentResult(),
            MapErrors
        );

    /// <summary>
    /// For successful read/query operations (GET)
    /// </summary>
    protected static IActionResult MapOkResult<T>(ErrorOr<T> result)
        => result.Match<IActionResult>(
            value => new OkObjectResult(value),
            MapErrors
        );

    /// <summary>
    /// For failed operations
    /// </summary>
    protected static IActionResult MapErrors(List<Error> errors)
    {
        var error = errors.First();

        return error.Type switch
        {
            ErrorType.Validation =>
                BuildProblem(error, StatusCodes.Status400BadRequest),

            ErrorType.Unauthorized =>
                new UnauthorizedResult(),

            ErrorType.NotFound =>
                BuildProblem(error, StatusCodes.Status404NotFound),

            ErrorType.Conflict =>
                BuildProblem(error, StatusCodes.Status409Conflict),

            _ => new ObjectResult(new ProblemDetails
            {
                Title = error.Code,
                Detail = error.Description,
                Status = StatusCodes.Status500InternalServerError,
                Type = "https://httpstatuses.com/500"
            })
            { StatusCode = StatusCodes.Status500InternalServerError }
        };
    }

    /// <summary>
    /// Builds a ProblemDetails object with the appropriate status code.
    /// </summary>
    private static IActionResult BuildProblem(Error error, int statusCode)
        => new ObjectResult(new ProblemDetails
        {
            Status = statusCode,
            Title = error.Code,
            Detail = error.Description,
            Type = $"https://httpstatuses.com/{statusCode}"
        })
        { StatusCode = statusCode };
}
