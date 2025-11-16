using Taskly.Core.BuildingBlocks.Domain;
using Taskly.Core.Functional;
using Vogen;

namespace Taskly.Domain.TaskItems;

[ValueObject<Guid>]
public readonly partial struct TaskItemId;

public sealed class TaskItem : Aggregate<TaskItemId>
{
    public const int MinTitleLength = 3;
    public const int MaxTitleLength = 100;
    
    public const int MaxDescriptionLength = 512;
    
    public string Title { get; private set; }
    public string? Description { get; private set; }
    public DateTime DueDate { get; private set; }
    public TaskItemPriority Priority { get; private set; }
    public bool IsCompleted { get; private set; }
    public string UserId { get; private set; }
    
    private TaskItem(string title, string description, DateTime dueDate, TaskItemPriority priority, string userId)
    {
        Id = TaskItemId.From(Guid.CreateVersion7());
        Title = title;
        Description = description;
        DueDate = dueDate;
        Priority = priority;
        IsCompleted = false;
        UserId = userId;
    }

    public static Result<TaskItem> TryCreate(string title, string description, DateTime dueDate, TaskItemPriority priority, string userId)
    {
        if (title.Length is > MaxTitleLength or < MinTitleLength)
        {
            return Error.Conflict("TaskItem.MaxTitleLength", $"The title can not be longer than {MaxTitleLength} characters or less  than {MinTitleLength} characters.");
        }

        if (description.Length is > MaxDescriptionLength)
        {
            return Error.Conflict("TaskItem.Description",  $"The description can not be longer than {MaxDescriptionLength} characters.");
        }

        if (dueDate < DateTime.Today)
        {
            return Error.Conflict("TaskItem.DueDate", $"The due date can only be in the future.");
        }

        if (string.IsNullOrEmpty(userId))
        {
            return Error.Conflict("TaskItem.UserId", $"The userId can not be null or empty.");
        }
        
        return new TaskItem(title, description, dueDate, priority, userId);
    }
    
    public void MarkCompleted()
    {
        if (!IsCompleted)
        {
            IsCompleted = true;
        }
    }
    
    public void ChangePriority(TaskItemPriority priority)
    {
        if (priority != Priority)
        {
            Priority = priority;
        }
    }
}