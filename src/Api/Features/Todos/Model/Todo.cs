using System.Diagnostics.CodeAnalysis;
using Api.Features.Shared.Models;
using Api.Features.Todos.Exceptions;
using Api.Features.Users.Model;

namespace Api.Features.Todos.Model;

[ValueObject<Guid>]
public readonly partial struct TodoId;

[SuppressMessage("ReSharper", "EntityFramework.ModelValidation.UnlimitedStringLength")]
public sealed class Todo : Entity<TodoId>
{
    public const int MinTitleLength = 3;
    public const int MaxTitleLength = 100;

    public const int MinDescriptionLength = 3;
    public const int MaxDescriptionLength = 512;


    public Todo(
        string title,
        string? description,
        TodoPriority priority,
        UserId userId)
    {
        Validate(title, description);

        Id = TodoId.From(Guid.CreateVersion7());
        Title = title;
        Description = description;
        Priority = priority;
        IsCompleted = false;
        UserId = userId;
    }

    public string Title { get; private set; }
    public string? Description { get; private set; }
    public TodoPriority Priority { get; private set; }
    public bool IsCompleted { get; private set; }
    public UserId UserId { get; private set; }
    public ICollection<Tag> Tags { get; init; } = [];
    public ICollection<Attachment> Attachments { get; init; } = [];

    public void Update(
        string title,
        string? description,
        TodoPriority priority)
    {
        Validate(title, description);

        Title = title;
        Description = description;
        Priority = priority;
    }

    private static void Validate(
        string title,
        string? description)
    {
        if (title.Length is > MaxTitleLength or < MinTitleLength)
        {
            throw new TodoInvalidTitleException(title.Length);
        }

        if (!string.IsNullOrEmpty(description) &&
            description.Length is > MaxDescriptionLength or < MinDescriptionLength)
        {
            throw new TodoInvalidDescriptionException(description.Length);
        }
    }

    public void SetCompletionStatus(
        bool isCompleted)
    {
        if (isCompleted != IsCompleted)
        {
            IsCompleted = isCompleted;
        }
    }

    public void ChangePriority(
        TodoPriority priority)
    {
        if (priority != Priority)
        {
            Priority = priority;
        }
    }
}