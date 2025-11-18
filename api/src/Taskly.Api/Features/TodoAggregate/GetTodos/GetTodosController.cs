using Mediator;
using Microsoft.AspNetCore.Mvc;
using Taskly.Api.Features.Shared;

namespace Taskly.Api.Features.TodoAggregate.GetTodos;

public sealed class GetTodosController(IMediator mediator) : TodosApiControllerBase
{
    [HttpGet("all")]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await mediator.Send(new GetTodosQuery(), ct);
        return MapOkResult(result);
    }
}