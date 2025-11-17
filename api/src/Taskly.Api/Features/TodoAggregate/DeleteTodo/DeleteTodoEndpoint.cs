using Mediator;
using Taskly.Api.Features.Shared;

namespace Taskly.Api.Features.TodoAggregate.DeleteTodo;

public sealed class DeleteTodoEndpoint : EndpointBase
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("todos/{id:guid}", async (
                    Guid id,
                    IMediator mediator,
                    CancellationToken ct) =>
                MapNoContentResult(await mediator.Send(new DeleteTodoCommand(id), ct)))
            .WithTags("Todos")
            .WithName("DeleteTodo")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);
    }
}