using Mediator;
using Microsoft.AspNetCore.Mvc;
using Taskly.Api.Features.Shared;

namespace Taskly.Api.Features.TodoAggregate.UpdateTodo;

public sealed class UpdateTodoEndpoint : EndpointBase
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("todos/{id:guid}", async (
                Guid id,
                IMediator mediator,
                [FromBody] UpdateTodoCommand command,
                CancellationToken ct) =>
            {
                command = command with { TodoId = id };
                var result = await mediator.Send(command, ct);
                return MapNoContentResult(result);
            })
            .WithTags("Todos")
            .WithName("UpdateTodo")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);
    }
}