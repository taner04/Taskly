using Mediator;
using Microsoft.AspNetCore.Mvc;
using Taskly.Api.Features.Shared;

namespace Taskly.Api.Features.TodoAggregate.DeleteTodo;

public sealed class DeleteTodoController(IMediator mediator) : TodosApiControllerBase
{
    [HttpDelete("delete")]
    public async Task<IActionResult> Delete([FromBody] DeleteTodoCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return MapNoContentResult(result);
    }
}