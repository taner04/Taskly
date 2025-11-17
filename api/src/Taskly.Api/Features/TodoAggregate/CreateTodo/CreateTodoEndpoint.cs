using Mediator;
using Microsoft.AspNetCore.Mvc;
using Taskly.Api.Features.Shared;

namespace Taskly.Api.Features.TodoAggregate.CreateTodo;

public sealed class CreateTodoEndpoint : EndpointBase
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("todos", async (
                IMediator mediator,
                [FromBody] CreateTodoCommand command,
                CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(command, cancellationToken);
                return MapOkResult(result);
            })
            .WithTags("Todos")
            .WithName("CreateTodo")
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);
    }
}