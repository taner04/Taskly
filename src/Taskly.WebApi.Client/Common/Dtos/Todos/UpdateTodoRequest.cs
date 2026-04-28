namespace Taskly.WebApi.Client.Common.Dtos.Todos;

public sealed record UpdateTodoRequest(string Title, string? Description, int Priority);