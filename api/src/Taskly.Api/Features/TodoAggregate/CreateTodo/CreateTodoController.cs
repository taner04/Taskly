using Mediator;
using Microsoft.AspNetCore.Mvc;
using Taskly.Api.Features.Shared;

namespace Taskly.Api.Features.TodoAggregate.CreateTodo;

public sealed class CreateTodoController(IMediator mediator) : TodosApiControllerBase
{
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateTodoCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);

        return MapNoContentResult(result);
    }
}