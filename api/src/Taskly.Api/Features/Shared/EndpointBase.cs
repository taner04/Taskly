using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using Taskly.Api.Abstractions;

namespace Taskly.Api.Features.Shared;

public abstract class EndpointBase : IEndpoint
{
    /// <summary>
    /// For successful update/delete operations (PUT/PATCH/DELETE)
    /// </summary>
    protected static IResult MapNoContentResult(ErrorOr<Success> result)
        => result.Match<IResult>(
            _ => Results.NoContent(),
            MapErrors);

    /// <summary>
    /// For successful read/query operations (GET)
    /// </summary>
    protected static IResult MapOkResult<T>(ErrorOr<T> result)
        => result.Match<IResult>(
            Results.Ok,
            MapErrors);

    
    private static IResult MapErrors(List<Error> errors)
    {
        var error = errors.First();

        return error.Type switch
        {
            ErrorType.Validation   => Results.BadRequest(BuildProblem(error, StatusCodes.Status400BadRequest)),
            ErrorType.Unauthorized => Results.Unauthorized(),
            ErrorType.NotFound     => Results.NotFound(BuildProblem(error, StatusCodes.Status404NotFound)),
            ErrorType.Conflict     => Results.Conflict(BuildProblem(error, StatusCodes.Status409Conflict)),
            _                      => Results.Problem(
                                         detail: error.Description,
                                         title: error.Code,
                                         statusCode: StatusCodes.Status500InternalServerError)
        };
    }

    private static ProblemDetails BuildProblem(Error error, int statusCode)
        => new()
        {
            Status = statusCode,
            Title = error.Code,
            Detail = error.Description,
            Type = $"https://httpstatuses.com/{statusCode}"
        };

    public abstract void MapEndpoint(IEndpointRouteBuilder app);
}
