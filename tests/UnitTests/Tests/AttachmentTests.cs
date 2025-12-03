using System.Text;
using Api.Features.Attachments.Models;
using Api.Features.Todos.Model;
using Microsoft.AspNetCore.Http;

namespace UnitTests.Tests;

public sealed class AttachmentTests
{
    private readonly TodoId _todoId = TodoId.From(Guid.NewGuid());

    private static FormFile CreateFakeFile(
        string fileName,
        string contentType = "application/octet-stream",
        int sizeInBytes = 100)
    {
        var bytes = Encoding.UTF8.GetBytes(new string('x', sizeInBytes));
        return new FormFile(new MemoryStream(bytes), 0, bytes.Length, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType
        };
    }

    [Fact]
    public void TryCreate_WithValidFile_ShouldReturnAttachment()
    {
        var file = CreateFakeFile("test.pdf", "application/pdf", 500);

        var result = Attachment.TryCreate(_todoId, file);

        Assert.False(result.IsError);

        var attachment = result.Value;

        Assert.Equal(_todoId, attachment.TodoId);
        Assert.Equal("test.pdf", attachment.FileName);
        Assert.Equal("application/pdf", attachment.ContentType);
        Assert.Equal(500, attachment.FileSize);

        Assert.StartsWith($"todo/{_todoId.Value}/", attachment.BlobName);
        Assert.EndsWith(".pdf", attachment.BlobName);

        Assert.Equal(Attachment.DefaultContainer, attachment.Container);
    }

    [Fact]
    public void TryCreate_WithFileNameTooLong_ShouldReturnError()
    {
        var longName = new string('a', Attachment.MaxFileNameLength + 1) + ".txt";
        var file = CreateFakeFile(longName);

        var result = Attachment.TryCreate(_todoId, file);

        Assert.True(result.IsError);
        Assert.Contains(result.Errors, e => e.Code == "Attachment.FileName");
    }

    [Fact]
    public void TryCreate_WithFileSizeTooLarge_ShouldReturnError()
    {
        // Just over 10 MB
        var file = CreateFakeFile("bigfile.txt", "text/plain", 10 * 1024 * 1024 + 1);

        var result = Attachment.TryCreate(_todoId, file);

        Assert.True(result.IsError);
        Assert.Contains(result.Errors, e => e.Code == "Attachment.FileSize");
    }

    [Theory]
    [InlineData("report.exe")]
    [InlineData("script.cs")]
    [InlineData("archive.zip")]
    [InlineData("virus.bat")]
    public void TryCreate_WithInvalidFileType_ShouldReturnError(
        string fileName)
    {
        var file = CreateFakeFile(fileName);

        var result = Attachment.TryCreate(_todoId, file);

        Assert.True(result.IsError);
        Assert.Contains(result.Errors, e => e.Code == "Attachment.FileType");
    }

    [Theory]
    [InlineData("file.txt")]
    [InlineData("image.jpg")]
    [InlineData("photo.jpeg")]
    [InlineData("document.pdf")]
    [InlineData("info.json")]
    [InlineData("logo.png")]
    public void TryCreate_WithValidExtensions_ShouldReturnSuccess(
        string fileName)
    {
        var file = CreateFakeFile(fileName);

        var result = Attachment.TryCreate(_todoId, file);

        Assert.False(result.IsError);
        Assert.Equal(Path.GetExtension(fileName).TrimStart('.'),
            Path.GetExtension(result.Value.BlobName).TrimStart('.'));
    }

    [Fact]
    public void TryCreate_ShouldNormalizeFileName()
    {
        var file = CreateFakeFile("weird///name..jpg", "image/jpeg");

        var result = Attachment.TryCreate(_todoId, file);

        Assert.False(result.IsError);
        Assert.Equal("name..jpg", result.Value.FileName);
    }

    [Fact]
    public void TryCreate_ShouldGenerateUniqueBlobName()
    {
        var file1 = CreateFakeFile("a.txt");
        var file2 = CreateFakeFile("a.txt");

        var r1 = Attachment.TryCreate(_todoId, file1).Value.BlobName;
        var r2 = Attachment.TryCreate(_todoId, file2).Value.BlobName;

        Assert.NotEqual(r1, r2);
    }

    [Fact]
    public void TryCreate_WhenExtensionMissing_ShouldReject()
    {
        var file = CreateFakeFile("noextension", "text/plain");

        var result = Attachment.TryCreate(_todoId, file);

        Assert.True(result.IsError);
        Assert.Contains(result.Errors, e => e.Code == "Attachment.FileType");
    }
}