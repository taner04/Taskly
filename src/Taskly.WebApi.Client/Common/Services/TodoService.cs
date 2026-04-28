using System.Net.Http.Headers;
using Microsoft.Extensions.Options;
using Taskly.Shared.Options;
using Taskly.Shared.WebApi.Responses.Todos;
using Taskly.WebApi.Client.Common.Dtos.Todos;
using Taskly.WebApi.Features.Attachments.WebHooks;

namespace Taskly.WebApi.Client.Common.Services;

public sealed class TodoService(
    IOptions<WebHookConfig> options,
    IHttpClientFactory httpClientFactory,
    IRefitWebApiClient webApiClient) : ITodoService
{
    private readonly WebHookConfig _webHookConfig = options.Value ?? throw new ArgumentNullException(nameof(options));

    public async Task<WebClientResult<GetTodoResponse>> AddAttachmentAsync(
        Guid todoId,
        AddAttachmentTodoRequest request,
        string filePath,
        CancellationToken cancellationToken)
    {
        var addAttachmentResult = await HttpOrchestrator.OrchestrateAsync<AddAttachmentResponse>(
            () => webApiClient.AddAttachmentAsync(todoId, request, cancellationToken), cancellationToken);

        if (!addAttachmentResult.IsSuccess)
        {
            return addAttachmentResult.Error;
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
            await webApiClient.SendAttachmentEventAsync(eventData, _webHookConfig.SecretKey, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return WebClientError.CustomError("Attachment.UploadFailed", "Failed to upload attachment",
                    "Could not upload the file to the cloud");
            }

            return addAttachmentResult.Value.TodoResponse;
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

    public async Task<WebClientResult<GetTodoResponse>> AddTagAsync(
        Guid todoId,
        AddTagsTodoRequest request,
        CancellationToken cancellationToken)
    {
        return await HttpOrchestrator.OrchestrateAsync<GetTodoResponse>(
            () => webApiClient.AddTagsToTodoAsync(todoId, request, cancellationToken),
            cancellationToken);
    }

    public async Task<WebClientResult<GetTodoResponse>> CompleteTodoAsync(
        Guid todoId,
        CompleteTodoRequest request,
        CancellationToken cancellationToken)
    {
        return await HttpOrchestrator.OrchestrateAsync<GetTodoResponse>(
            () => webApiClient.CompleteTodoAsync(todoId, request, cancellationToken),
            cancellationToken);
    }

    public async Task<WebClientResult<GetTodoResponse>> CreateTodoAsync(
        CreateTodoRequest request,
        CancellationToken cancellationToken)
    {
        return await HttpOrchestrator.OrchestrateAsync<GetTodoResponse>(
            () => webApiClient.CreateTodoAsync(request, cancellationToken),
            cancellationToken);
    }

    public async Task<WebClientResult<GetTodoResponse>> DeleteTodoAsync(
        Guid todoId,
        CancellationToken cancellationToken)
    {
        return await HttpOrchestrator.OrchestrateAsync<GetTodoResponse>(
            () => webApiClient.DeleteTodoAsync(todoId, cancellationToken),
            cancellationToken);
    }

    public async Task<WebClientResult<PaginationResult<GetTodoResponse>>> GetTodosAsync(
        PaginationQuery query,
        CancellationToken cancellationToken = default)
    {
        return await HttpOrchestrator.OrchestrateAsync<PaginationResult<GetTodoResponse>>(
            () => webApiClient.GetTodosAsync(query, cancellationToken), cancellationToken);
    }

    public async Task<WebClientResult<GetTodoResponse>> RemoveAttachmentAsync(
        Guid todoId,
        Guid attachmentId,
        CancellationToken cancellationToken)
    {
        return await HttpOrchestrator.OrchestrateAsync<GetTodoResponse>(
            () => webApiClient.RemoveAttachmentAsync(todoId, attachmentId, cancellationToken), cancellationToken);
    }

    public async Task<WebClientResult<GetTodoResponse>> RemoveReminderAsync(
        Guid todoId,
        CancellationToken cancellationToken)
    {
        return await HttpOrchestrator.OrchestrateAsync<GetTodoResponse>(
            () => webApiClient.RemoveReminderAsync(todoId, cancellationToken),
            cancellationToken);
    }

    public async Task<WebClientResult<GetTodoResponse>> RemoveTagAsync(
        Guid todoId,
        Guid tagId,
        CancellationToken cancellationToken)
    {
        return await HttpOrchestrator.OrchestrateAsync<GetTodoResponse>(
            () => webApiClient.RemoveTagFromTodoAsync(todoId, tagId, cancellationToken),
            cancellationToken);
    }

    public async Task<WebClientResult<GetTodoResponse>> UpdateReminderAsync(
        Guid todoId,
        UpdateReminderRequest request,
        CancellationToken cancellationToken)
    {
        return await HttpOrchestrator.OrchestrateAsync<GetTodoResponse>(
            () => webApiClient.UpdateReminderAsync(todoId, request, cancellationToken),
            cancellationToken);
    }

    public async Task<WebClientResult<GetTodoResponse>> UpdateTodoAsync(
        Guid todoId,
        UpdateTodoRequest request,
        CancellationToken cancellationToken)
    {
        return await HttpOrchestrator.OrchestrateAsync<GetTodoResponse>(
            () => webApiClient.UpdateTodoAsync(todoId, request, cancellationToken),
            cancellationToken);
    }
}