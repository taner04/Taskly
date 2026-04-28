using Taskly.Shared.WebApi.Responses.Tags;

namespace Taskly.WebApi.Features.Todos.Common.Extensions;

internal static class TodoExtensions
{
    extension(Todo todo)
    {
        public GetTodoResponse ToGetTodoResponse()
        {
            return new GetTodoResponse(
                todo.Id.Value,
                todo.Title,
                todo.Description,
                (int)todo.Priority,
                todo.IsCompleted,
                todo.CreatedAt,
                todo.Tags.Select(t => new GetTagResponse(t.Id.Value,
                        t.Name))
                    .ToList(),
                todo.Attachments.Select(a => new GetTodoAttachments(a.Id.Value,
                        a.FileName,
                        a.FileSize,
                        a.ContentType))
                    .ToList());
        }
    }
}