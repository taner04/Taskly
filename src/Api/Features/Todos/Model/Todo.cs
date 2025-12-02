using Api.Features.Shared.Domain;
using Api.Features.Tags.Model;

namespace Api.Features.Todos.Model;

[ValueObject<Guid>]
public readonly partial struct TodoId;

public sealed class Todo : Entity<TodoId>
{
    public const int MinTitleLength = 3;
    public const int MaxTitleLength = 100;

    public const int MinDescriptionLength = 3;
    public const int MaxDescriptionLength = 512;

    public const int MaxUserIdLength = 256;

    private Todo(
        string title,
        string? description,
        TodoPriority priority,
        string userId)
    {
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
    public string UserId { get; private set; }
    public ICollection<Tag> Tags { get; init; } = [];

    public static ErrorOr<Todo> TryCreate(
        string title,
        string? description,
        TodoPriority priority,
        string userId)
    {
        if (title.Length is > MaxTitleLength or < MinTitleLength)
        {
            return Error.Conflict("Todo.MaxTitleLength",
                $"The title can not be longer than {MaxTitleLength} characters or less than {MinTitleLength} characters.");
        }

        if (!string.IsNullOrEmpty(description) &&
            description.Length is > MaxDescriptionLength or < MinDescriptionLength)
        {
            return Error.Conflict("Todo.Description",
                $"The description can not be longer than {MaxDescriptionLength} characters or less then {MinDescriptionLength} characters.");
        }

        if (string.IsNullOrEmpty(userId) || userId.Length > MaxUserIdLength)
        {
            return Error.Conflict("Todo.UserId", "The userId can not be null or empty.");
        }

        return new Todo(title, description, priority, userId);
    }

    public ErrorOr<Success> Update(
        string title,
        string? description,
        TodoPriority priority)
    {
        if (title.Length is > MaxTitleLength or < MinTitleLength)
        {
            return Error.Conflict("Todo.MaxTitleLength",
                $"The title can not be longer than {MaxTitleLength} characters or less than {MinTitleLength} characters.");
        }

        if (!string.IsNullOrEmpty(description) &&
            description.Length is > MaxDescriptionLength or < MinDescriptionLength)
        {
            return Error.Conflict("Todo.Description",
                $"The description can not be longer than {MaxDescriptionLength} characters or less then {MinDescriptionLength} characters.");
        }

        Title = title;
        Description = description;
        Priority = priority;

        return Result.Success;
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