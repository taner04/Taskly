using System.Diagnostics.CodeAnalysis;
using Taskly.WebApi.Common.Shared.Models;
using Taskly.WebApi.Features.Attachments.Exceptions;
using Taskly.WebApi.Features.Todos.Models;
using TodoId = Taskly.WebApi.Features.Todos.Models.TodoId;

namespace Taskly.WebApi.Features.Attachments.Models;

[ValueObject<Guid>]
public readonly partial struct AttachmentId;

public enum AttachmentStatus
{
    Pending = 0,
    Uploaded = 1
}

[SuppressMessage("ReSharper", "EntityFramework.ModelValidation.UnlimitedStringLength")]
public sealed class Attachment : Entity<AttachmentId>
{
    public const long MaxFileSizeInBytes = 10 * 1024 * 1024; // 10MB
    public const int MaxFileNameLength = 255;
    public const string DefaultContainer = "attachments";

    private static readonly HashSet<string> AllowedFileTypes =
        ["json", "txt", "pdf", "png", "jpg", "jpeg", "docx", "pptx", "xlsx"];

    private Attachment(
        TodoId todoId,
        string fileName,
        string blobName,
        string contentType)
    {
        Id = AttachmentId.From(Guid.CreateVersion7());
        TodoId = todoId;
        FileName = fileName;
        BlobName = blobName;
        ContentType = contentType;
        Container = DefaultContainer;
        Status = AttachmentStatus.Pending;
    }

    public TodoId TodoId { get; init; }
    public Todo Todo { get; init; } = null!;

    public string FileName { get; private set; }
    public string BlobName { get; private set; }
    public string ContentType { get; private set; }
    public long FileSize { get; private set; }
    public string Container { get; private set; }
    public AttachmentStatus Status { get; private set; }

    public static Attachment CreatePending(
        TodoId todoId,
        string fileName,
        string contentType)
    {
        var extension = Path.GetExtension(fileName)
            .TrimStart('.')
            .ToLowerInvariant();

        if (!AllowedFileTypes.Contains(extension))
        {
            throw new AttachmentInvalidExtensionException(extension);
        }

        var blobName = $"todo/{todoId.Value}/{Guid.NewGuid()}.{extension}";

        return new Attachment(
            todoId,
            Path.GetFileName(fileName),
            blobName,
            contentType
        );
    }

    public void MarkUploaded(
        long fileSize)
    {
        if (fileSize is <= 0 or > MaxFileSizeInBytes)
        {
            throw new AttachmentInvalidFileSizeException(fileSize);
        }

        FileSize = fileSize;
        Status = AttachmentStatus.Uploaded;
    }

    public string GetDownloadUrl() => $"/attachments/{Id.Value}/download";
}