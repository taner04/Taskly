namespace Taskly.WebApi.Client.Common.Dtos.Todos;

public sealed record CreateTodoRequest(string Title, string? Description, int Priority);