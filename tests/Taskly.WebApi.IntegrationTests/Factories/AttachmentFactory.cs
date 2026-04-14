namespace Taskly.WebApi.IntegrationTests.Factories;

internal static class AttachmentFactory
{
    internal static Attachment CreatePending(
        Todo todo,
        string fileName = "testfile.txt",
        string contentType = "text/plain") =>
        Attachment.Create(todo.Id, fileName, contentType);

    internal static Attachment CreateUploaded(
        Todo todo,
        long fileSize = 1014,
        string fileName = "testfile.txt",
        string contentType = "text/plain")
    {
        var attachment = CreatePending(todo, fileName, contentType);
        attachment.FileSize = fileSize;
        attachment.Status = AttachmentStatus.Uploaded;

        return attachment;
    }
}