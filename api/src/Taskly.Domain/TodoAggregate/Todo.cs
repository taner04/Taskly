using ErrorOr;
using Taskly.Core.BuildingBlocks.Domain;
using Vogen;

namespace Taskly.Domain.TodoAggregate;

[ValueObject<Guid>]
public readonly partial struct TodoId;

public sealed class Todo : Aggregate<TodoId>
{
    public const int MinTitleLength = 3;
    public const int MaxTitleLength = 100;

    public const int MaxDescriptionLength = 512;

    private Todo(string title, string description, DateTime dueDate, TodoPriority priority, string userId)
    {
        Id = TodoId.From(Guid.CreateVersion7());
        Title = title;
        Description = description;
        DueDate = dueDate;
        Priority = priority;
        IsCompleted = false;
        UserId = userId;
    }

    public string Title { get; private set; }
    public string? Description { get; private set; }
    public DateTime DueDate { get; private set; }
    public TodoPriority Priority { get; private set; }
    public bool IsCompleted { get; private set; }
    public string UserId { get; private set; }

    private static ErrorOr<Success> Validate(string title, string? description, DateTime dueDate)
    {
        if (title.Length is > MaxTitleLength or < MinTitleLength)
            return Error.Conflict("Todo.MaxTitleLength",
                $"The title can not be longer than {MaxTitleLength} characters or less than {MinTitleLength} characters.");

        if (!string.IsNullOrEmpty(description) && description.Length > MaxDescriptionLength)
            return Error.Conflict("Todo.Description",
                $"The description can not be longer than {MaxDescriptionLength} characters.");

        if (dueDate < DateTime.Today)
            return Error.Conflict("TaskItem.DueDate", "The due date can only be in the future.");

        return Result.Success;
    }


    public static ErrorOr<Todo> TryCreate(string title, string description, DateTime dueDate, TodoPriority priority,
        string userId)
    {
        var validationResult = Validate(title, description, dueDate);
        if (validationResult.IsError) return validationResult.Errors;

        if (string.IsNullOrEmpty(userId)) return Error.Conflict("Todo.UserId", "The userId can not be null or empty.");

        return new Todo(title, description, dueDate, priority, userId);
    }

    public ErrorOr<Success> Update(string title, string? description, DateTime dueDate, TodoPriority priority,
        bool isCompleted)
    {
        var validationResult = Validate(title, description, dueDate);
        if (validationResult.IsError) return validationResult;

        Title = title;
        Description = description;
        DueDate = dueDate;
        Priority = priority;
        IsCompleted = isCompleted;

        return Result.Success;
    }

    public void ChangePriority(TodoPriority priority)
    {
        if (priority != Priority) Priority = priority;
    }
}