namespace Taskly.WebApi.Features.Attachments.Common.Exceptions;

internal sealed class InvalidAttachmentFileSizeException :
    TasklyException
{
    private InvalidAttachmentFileSizeException(long fileSize) : base("Invalid attachment file size.",
        $"Attachment file size '{fileSize}' exceeds the maximum allowed size.",
        "Attachment.InvalidFileSize",
        HttpStatusCode.BadRequest)
    {
    }

    public static void ThrowInvalid(long fileSize)
    {
        if (fileSize is > Attachment.MaxFileSizeInBytes or <= 0)
        {
            throw new InvalidAttachmentFileSizeException(fileSize);
        }
    }
}