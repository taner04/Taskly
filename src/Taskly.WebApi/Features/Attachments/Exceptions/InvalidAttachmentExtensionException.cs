namespace Taskly.WebApi.Features.Attachments.Exceptions;

internal sealed class InvalidAttachmentExtensionException :
    TasklyException
{
    private static readonly HashSet<string> AllowedFileTypes =
        ["json", "txt", "pdf", "png", "jpg", "jpeg", "docx", "pptx", "xlsx"];

    private InvalidAttachmentExtensionException(string extension) : base("The used file extension is not allowed.",
        $"The file extension '{extension}' is not permitted for attachments.",
        "Attachment.InvalidExtension",
        HttpStatusCode.BadRequest)
    {
    }

    public static void ThrowIfInvalidExtension(string extension)
    {
        if (!AllowedFileTypes.Contains(extension.ToLowerInvariant()))
        {
            throw new InvalidAttachmentExtensionException(extension);
        }
    }
}