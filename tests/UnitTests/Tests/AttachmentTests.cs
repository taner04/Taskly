using Api.Features.Attachments.Exceptions;
using Api.Features.Attachments.Models;
using Api.Features.Todos.Model;

namespace UnitTests.Tests;

public sealed class AttachmentTests
{
    private const string ValidFileName = "file.pdf";
    private const string ValidContentType = "application/pdf";

    private const string InvalidFileName = "file.exe";
    private const long ValidFileSize = 1024;
    private const long TooLargeFileSize = AttachmentTestsMaxSize + 1;

    private const long AttachmentTestsMaxSize = 10 * 1024 * 1024; // 10MB from entity
    private readonly TodoId _todoId = TodoId.From(Guid.NewGuid());

    [Fact]
    public void Create_WithValidValues_ShouldReturnAttachment()
    {
        var attachment = Attachment.CreatePending(_todoId, ValidFileName, ValidContentType);

        Assert.Equal(_todoId, attachment.TodoId);
        Assert.Equal("file.pdf", attachment.FileName);
        Assert.Equal(ValidContentType, attachment.ContentType);

        Assert.StartsWith($"todo/{_todoId.Value}/", attachment.BlobName);
        Assert.EndsWith(".pdf", attachment.BlobName);

        Assert.Equal(AttachmentStatus.Pending, attachment.Status);
        Assert.Equal(Attachment.DefaultContainer, attachment.Container);

        Assert.NotEqual(Guid.Empty, attachment.Id.Value);
    }

    [Fact]
    public void Create_WithInvalidExtension_ShouldThrow()
    {
        Assert.Throws<AttachmentInvalidExtensionException>(() =>
            Attachment.CreatePending(_todoId, InvalidFileName, "application/octet-stream"));
    }

    [Theory]
    [InlineData("text.txt")]
    [InlineData("pic.jpg")]
    [InlineData("photo.jpeg")]
    [InlineData("scan.png")]
    [InlineData("doc.pdf")]
    [InlineData("data.json")]
    public void Create_WithAllowedExtensions_ShouldNotThrow(
        string fileName)
    {
        var attachment = Attachment.CreatePending(_todoId, fileName, "application/test");

        Assert.Equal(Path.GetExtension(fileName).TrimStart('.'),
            Path.GetExtension(attachment.BlobName).TrimStart('.'));
    }

    [Fact]
    public void MarkUploaded_WithValidFileSize_ShouldSetStatusAndFileSize()
    {
        var attachment = Attachment.CreatePending(_todoId, ValidFileName, ValidContentType);

        attachment.MarkUploaded(ValidFileSize);

        Assert.Equal(ValidFileSize, attachment.FileSize);
        Assert.Equal(AttachmentStatus.Uploaded, attachment.Status);
    }

    [Fact]
    public void MarkUploaded_WithZeroFileSize_ShouldThrow()
    {
        var attachment = Attachment.CreatePending(_todoId, ValidFileName, ValidContentType);

        Assert.Throws<AttachmentInvalidFileSizeException>(() => attachment.MarkUploaded(0));
    }

    [Fact]
    public void MarkUploaded_WithNegativeFileSize_ShouldThrow()
    {
        var attachment = Attachment.CreatePending(_todoId, ValidFileName, ValidContentType);

        Assert.Throws<AttachmentInvalidFileSizeException>(() => attachment.MarkUploaded(-1));
    }

    [Fact]
    public void MarkUploaded_WithFileSizeTooLarge_ShouldThrow()
    {
        var attachment = Attachment.CreatePending(_todoId, ValidFileName, ValidContentType);

        Assert.Throws<AttachmentInvalidFileSizeException>(() => attachment.MarkUploaded(TooLargeFileSize));
    }

    [Fact]
    public void GetDownloadUrl_ShouldReturnExpectedFormat()
    {
        var attachment = Attachment.CreatePending(_todoId, ValidFileName, ValidContentType);

        var url = attachment.GetDownloadUrl();

        Assert.Equal($"/attachments/{attachment.Id.Value}/download", url);
    }

    [Fact]
    public void Create_ShouldNormalizeFileName()
    {
        var attachment = Attachment.CreatePending(_todoId, "weird///name..jpg", "image/jpeg");

        Assert.Equal("name..jpg", attachment.FileName);
    }

    [Fact]
    public void Create_ShouldGenerateUniqueBlobNames()
    {
        var a1 = Attachment.CreatePending(_todoId, "file.pdf", "application/pdf").BlobName;
        var a2 = Attachment.CreatePending(_todoId, "file.pdf", "application/pdf").BlobName;

        Assert.NotEqual(a1, a2);
    }
}