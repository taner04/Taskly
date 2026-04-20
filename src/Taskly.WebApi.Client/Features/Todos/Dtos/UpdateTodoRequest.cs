namespace Taskly.WebApi.Client.Features.Todos.Dtos;

public sealed record UpdateTodoRequest(string Title, string? Description, int Priority);