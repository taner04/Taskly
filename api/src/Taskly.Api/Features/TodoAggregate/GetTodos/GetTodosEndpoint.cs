using Mediator;
using Taskly.Api.Features.Shared;

namespace Taskly.Api.Features.TodoAggregate.GetTodos;

public sealed class GetTodosEndpoint : EndpointBase
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("todos", async (IMediator mediator, CancellationToken ct) =>
                MapOkResult(await mediator.Send(new GetTodosQuery(), ct)))
            .WithTags("Todos")
            .WithName("GetTodos")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
    }
}