using System.Net.Http.Headers;
using Taskly.Shared.Pagination;
using Taskly.WebApi.Client.Features.Todos.Dtos;
using Taskly.WebApi.Features.Attachments.WebHooks;
using Taskly.WebApi.Features.Todos.Endpoints;

namespace Taskly.WebApi.Client.Features.Todos.Services;

public sealed class TodoService(
    IHttpClientFactory httpClientFactory,
    IRefitWebApiClient webApiClient) : EndpointService
{
    public async Task<WebClientResult> AddAttachmentAsync(
        Guid todoId,
        AddAttachmentTodoRequest request,
        string filePath,
        CancellationToken cancellationToken)
    {
        var addAttachmentResult = await HttpCallOrchestrator<AddAttachment.Response>(
            () => webApiClient.AddAttachmentAsync(todoId, request, cancellationToken), cancellationToken);
        
        if (!addAttachmentResult.IsSuccess)
        {
            return addAttachmentResult;
        }

        try
        {
            using var httpClient = httpClientFactory.CreateClient();
            await using var fileStream = File.OpenRead(filePath);

            var content = new StreamContent(fileStream);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            var response = await httpClient.PutAsync(addAttachmentResult.Value.UploadUrl, content, cancellationToken);

            AttachmentEventData eventData = response.IsSuccessStatusCode
                ? new AttachmentEventData.Completed(fileStream.Length)
                : new AttachmentEventData.Failed();
            await webApiClient.SendAttachmentEventAsync(eventData, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return WebClientError.CustomError("Attachment.UploadFailed", "Failed to upload attachment",
                    "Could not upload the file to the cloud");
            }

            return WebClientResult.Success();
        }
        catch (FileNotFoundException ex)
        {
            return WebClientError.CustomError(
                "Attachment.FileNotFound",
                "Failed to upload attachment",
                $"File not found: {ex.Message}");
        }
        catch (UnauthorizedAccessException ex)
        {
            return WebClientError.CustomError(
                "Attachment.AccessDenied",
                "Failed to upload attachment",
                $"Access denied: {ex.Message}");
        }
        catch (HttpRequestException ex)
        {
            return WebClientError.CustomError(
                "Attachment.UploadFailed",
                "Failed to upload attachment",
                $"Upload error: {ex.Message}");
        }
        catch (OperationCanceledException)
        {
            return WebClientError.CustomError(
                "Attachment.Cancelled",
                "Upload cancelled",
                "The operation was cancelled");
        }
        catch (Exception ex)
        {
            return WebClientError.CustomError(
                "Attachment.UnexpectedError",
                "Unexpected error while uploading attachment",
                ex.Message);
        }
    }

    public async Task<WebClientResult> AddTagAsync(
        Guid todoId,
        AddTagsTodoRequest request,
        CancellationToken cancellationToken)
    {
        return await HttpCallOrchestrator(() => webApiClient.AddTagsToTodoAsync(todoId, request, cancellationToken),
            cancellationToken);
    }

    public async Task<WebClientResult> CompleteTodoAsync(
        Guid todoId,
        CompleteTodoRequest request,
        CancellationToken cancellationToken)
    {
        return await HttpCallOrchestrator(() => webApiClient.CompleteTodoAsync(todoId, request, cancellationToken),
            cancellationToken);
    }

    public async Task<WebClientResult> CreateTodoAsync(CreateTodoRequest request, CancellationToken cancellationToken)
    {
        return await HttpCallOrchestrator(() => webApiClient.CreateTodoAsync(request, cancellationToken),
            cancellationToken);
    }

    public async Task<WebClientResult> DeleteTodoAsync(Guid todoId, CancellationToken cancellationToken)
    {
        return await HttpCallOrchestrator(() => webApiClient.DeleteTodoAsync(todoId, cancellationToken),
            cancellationToken);
    }

    public async Task<WebClientResult<PaginationResult<GetTodos.Response>>> GetTodosAsync(
        PaginationQuery query,
        CancellationToken cancellationToken)
    {
        return await HttpCallOrchestrator<PaginationResult<GetTodos.Response>>(
            () => webApiClient.GetTodosAsync(query, cancellationToken), cancellationToken);
    }

    public async Task<WebClientResult> RemoveAttachmentAsync(
        Guid todoId,
        Guid attachmentId,
        CancellationToken cancellationToken)
    {
        return await HttpCallOrchestrator(
            () => webApiClient.RemoveAttachmentAsync(todoId, attachmentId, cancellationToken), cancellationToken);
    }

    public async Task<WebClientResult> RemoveReminderAsync(Guid todoId, CancellationToken cancellationToken)
    {
        return await HttpCallOrchestrator(() => webApiClient.RemoveReminderAsync(todoId, cancellationToken),
            cancellationToken);
    }

    public async Task<WebClientResult> RemoveTagAsync(Guid todoId, Guid tagId, CancellationToken cancellationToken)
    {
        return await HttpCallOrchestrator(() => webApiClient.RemoveTagFromTodoAsync(todoId, tagId, cancellationToken),
            cancellationToken);
    }

    public async Task<WebClientResult> UpdateReminderAsync(
        Guid todoId,
        UpdateReminderRequest request,
        CancellationToken cancellationToken)
    {
        return await HttpCallOrchestrator(() => webApiClient.UpdateReminderAsync(todoId, request, cancellationToken),
            cancellationToken);
    }

    public async Task<WebClientResult> UpdateTodoAsync(
        Guid todoId,
        UpdateTodoRequest request,
        CancellationToken cancellationToken)
    {
        return await HttpCallOrchestrator(() => webApiClient.UpdateTodoAsync(todoId, request, cancellationToken),
            cancellationToken);
    }
}