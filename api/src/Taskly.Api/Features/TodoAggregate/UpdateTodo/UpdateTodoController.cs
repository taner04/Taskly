using Mediator;
using Microsoft.AspNetCore.Mvc;
using Taskly.Api.Features.Shared;

namespace Taskly.Api.Features.TodoAggregate.UpdateTodo;

public sealed class UpdateTodoController(IMediator mediator) : TodosApiControllerBase
{
    [HttpPut("update")]
    public async Task<IActionResult> Update([FromBody] UpdateTodoCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return MapNoContentResult(result);
    }
}