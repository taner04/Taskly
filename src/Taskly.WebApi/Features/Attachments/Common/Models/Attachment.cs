using Taskly.ServiceDefaults;
using Taskly.WebApi.Common.Shared.Models;
using Taskly.WebApi.Features.Attachments.Common.Exceptions;

namespace Taskly.WebApi.Features.Attachments.Common.Models;

[ValueObject<Guid>]
public readonly partial struct AttachmentId;

public enum AttachmentStatus
{
    Pending = 0,
    Uploaded = 1
}

[Validate]
public sealed partial class Attachment : Entity<AttachmentId>, IValidationTarget<Attachment>
{
    public const long MaxFileSizeInBytes = 10 * 1024 * 1024; // 10MB
    public const int MaxFileNameLength = 255;
    private const string BlobContainerName = AppHostConstants.AzureBlobContainerName;

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
        Container = BlobContainerName;
        Status = AttachmentStatus.Pending;
    }

    public TodoId TodoId { get; init; }
    public Todo Todo { get; init; } = null!;

    public string FileName { get; init; }
    public string BlobName { get; init; }
    public string ContentType { get; init; }
    public long FileSize { get; set; }
    public string Container { get; init; }
    public AttachmentStatus Status { get; set; }

    public static Attachment Create(
        TodoId todoId,
        string fileName,
        string contentType)
    {
        var extension = Path.GetExtension(fileName)
            .TrimStart('.')
            .ToLowerInvariant();

        InvalidAttachmentExtensionException.ThrowIfInvalidExtension(extension);


        var blobName = $"todo/{todoId.Value}/{Guid.NewGuid()}.{extension}";

        return new Attachment(
            todoId,
            Path.GetFileName(fileName),
            blobName,
            contentType
        );
    }
}