using System.Diagnostics.CodeAnalysis;
using Taskly.WebApi.Common.Shared.Models;
using Taskly.WebApi.Features.Todos.Exceptions;

namespace Taskly.WebApi.Features.Todos.Models;

[ValueObject<Guid>]
public readonly partial struct TodoId;

[SuppressMessage("ReSharper", "EntityFramework.ModelValidation.UnlimitedStringLength")]
public sealed class Todo : Entity<TodoId>
{
    public const int MinTitleLength = 3;
    public const int MaxTitleLength = 100;

    public const int MinDescriptionLength = 3;
    public const int MaxDescriptionLength = 512;

    private Todo(
        string title,
        string? description,
        TodoPriority priority,
        UserId userId)
    {
        Id = TodoId.From(Guid.CreateVersion7());
        Title = title;
        Description = description;
        Priority = priority;
        IsCompleted = false;
        UserId = userId;
    }

    public string Title { get; set; }
    public string? Description { get; set; }
    public TodoPriority Priority { get; set; }
    public bool IsCompleted { get; set; }
    public UserId UserId { get; init; }
    public DateTime? Deadline { get; set; }
    public int? ReminderOffsetInMinutes { get; set; }
    public string? HangfireJobId { get; set; }

    public DateTime? ReminderAt
        => Deadline.HasValue &&
           ReminderOffsetInMinutes.HasValue
            ? Deadline.Value.AddMinutes(-ReminderOffsetInMinutes.Value)
            : null;

    public List<Tag> Tags { get; private set; } = [];
    public List<Attachment> Attachments { get; private set; } = [];
    public User User { get; private set; } = null!;

    public static Todo Create(string title, string? description, TodoPriority priority, UserId userId)
    {
        InvalidTodoTitleException.ThrowIfInvalid(title);
        InvalidTodoDescriptionException.ThrowIfInvalid(description);

        return new Todo(title, description, priority, userId);
    }
}