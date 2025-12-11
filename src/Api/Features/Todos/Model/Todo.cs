using System.Diagnostics.CodeAnalysis;
using Api.Features.Shared.Extensions;
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

    private readonly List<Tag> _tags = [];
    private readonly List<Attachment> _attachments = [];
    
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

    public string Title { get; private set; }
    public string? Description { get; private set; }
    public TodoPriority Priority { get; private set; }
    public bool IsCompleted { get; private set; }
    public UserId UserId { get; private set; }
    public DateTime? Deadline { get; private set; }
    public int? ReminderOffsetInMinutes { get; private set; }

    public DateTime? ReminderAt
        => Deadline.HasValue &&
           ReminderOffsetInMinutes.HasValue
            ? Deadline.Value.AddMinutes(-ReminderOffsetInMinutes.Value)
            : null;

    public List<Tag> Tags { get; private set; } = [];
    public List<Attachment> Attachments { get; private set; } = [];

    public static Todo Create(
        string title,
        string? description,
        TodoPriority priority,
        UserId userId)
    {
        Validate(title, description);

        return new Todo(title, description, priority, userId);
    }

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
        title.EnsureLengthInRange<Todo>(MinTitleLength, MaxTitleLength, nameof(Title));
        description?.EnsureLengthInRange<Todo>(MinDescriptionLength, MaxDescriptionLength, nameof(Description));
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

    public void ClearReminder()
    {
        Deadline = null;
        ReminderOffsetInMinutes = null;
    }

    public void SetReminder(
        DateTime deadline,
        int reminder)
    {
        if (deadline <= DateTime.UtcNow)
        {
            throw new TodoInvalidDeadlineException(
                deadline,
                reminder,
                "Deadline must be set to a future date and time.");
        }

        if (reminder < 0)
        {
            throw new TodoInvalidDeadlineException(
                deadline,
                reminder,
                "Reminder minutes cannot be negative.");
        }

        var reminderAt = deadline.AddMinutes(-reminder);
        if (reminderAt > deadline)
        {
            throw new TodoInvalidDeadlineException(
                deadline,
                reminder,
                "Reminder cannot occur after the deadline.");
        }

        if (reminder > (deadline - DateTime.UtcNow).TotalMinutes)
        {
            throw new TodoInvalidDeadlineException(
                deadline,
                reminder,
                "Reminder cannot be further in the past than the time until deadline.");
        }

        Deadline = deadline;
        ReminderOffsetInMinutes = reminder;
    }
}