using Microsoft.AspNetCore.Mvc;
using Taskly.Api.Attributes;
using Taskly.Api.Features.Shared;

namespace Taskly.Api.Features.TodoAggregate;

[ApiController]
[Route("api/todos")]
[ControllerGroup("Todos")]
public abstract class TodosApiControllerBase : ApiControllerBase;