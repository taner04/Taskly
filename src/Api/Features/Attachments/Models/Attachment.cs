using System.Diagnostics.CodeAnalysis;
using Api.Features.Todos.Model;
using Api.Shared.Features.Models;

namespace Api.Features.Attachments.Models;

[ValueObject<Guid>]
public readonly partial struct AttachmentId;

[SuppressMessage("ReSharper", "EntityFramework.ModelValidation.UnlimitedStringLength")]
public sealed class Attachment : Entity<AttachmentId>
{
    private const long MaxFileSizeInBytes = 10 * 1024 * 1024; // 10MB

    public const int MaxFileNameLength = 255;
    public const string DefaultContainer = "attachments";

    private static readonly HashSet<string> AllowedFileTypes = ["json", "txt", "pdf", "png", "jpg", "jpeg"];

    private Attachment(
        TodoId todoId,
        string fileName,
        string blobName,
        string contentType,
        long fileSize)
    {
        TodoId = todoId;
        FileName = fileName;
        BlobName = blobName;
        ContentType = contentType;
        FileSize = fileSize;
        Container = DefaultContainer;
    }

    public TodoId TodoId { get; init; }
    public Todo Todo { get; init; } = null!;

    public string FileName { get; init; } 
    public string BlobName { get; init; }
    public string ContentType { get; init; }
    public long FileSize { get; init; }
    public string Container { get; init; }

    public static ErrorOr<Attachment> TryCreate(
        TodoId todoId,
        IFormFile file)
    {
        if (file.Length > MaxFileSizeInBytes)
        {
            return Error.Conflict("Attachment.FileSize",
                $"The file size cannot exceed {MaxFileSizeInBytes} Bytes.");
        }

        if (file.FileName.Length > MaxFileNameLength)
        {
            return Error.Conflict("Attachment.FileName",
                $"The file name cannot exceed {MaxFileNameLength} characters.");
        }

        var extension = Path.GetExtension(file.FileName)
            .TrimStart('.')
            .ToLowerInvariant();

        if (!AllowedFileTypes.Contains(extension))
        {
            return Error.Conflict("Attachment.FileType",
                $"File type '{extension}' is not allowed. Allowed types: {string.Join(", ", AllowedFileTypes)}.");
        }
        
        var blobName = $"todo/{todoId.Value}/{Guid.NewGuid()}.{extension}";

        return new Attachment(
            todoId,
            Path.GetFileName(file.FileName),
            blobName,
            file.ContentType,
            file.Length
        );
    }
}