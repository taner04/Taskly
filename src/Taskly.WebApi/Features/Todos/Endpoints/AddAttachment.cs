using Microsoft.AspNetCore.Http.HttpResults;
using Taskly.WebApi.Features.Attachments.Services;
using Taskly.WebApi.Features.Todos.Specifications;

namespace Taskly.WebApi.Features.Todos.Endpoints;

//TODO: Add support for multiple files
[Handler]
[MapPost(ApiRoutes.Todos.AddAttachment)]
[Authorize(Policy = Security.Policies.User)]
public static partial class AddAttachment
{
    internal static void CustomizeEndpoint(
        RouteHandlerBuilder endpoint)
    {
        endpoint.WithTags(nameof(Todo));
        endpoint.RequireRateLimiting(Security.RateLimiting.Global);
    }

    internal static Ok<Response> TransformResult(
        Response response) =>
        TypedResults.Ok(response);

    private static async ValueTask<Response> HandleAsync(
        [AsParameters] Command command,
        TasklyDbContext context,
        CurrentUserService currentUserService,
        AttachmentBlobContainerService attachmentBlobContainerService,
        CancellationToken ct)
    {
        var userId = currentUserService.GetUserId();

        var spec = new TodoByUserIdSpecificationWithAttachmentsSpec(command.TodoId, userId);
        var todo = await context.Todos
            .WithSpecification(spec)
            .SingleOrDefaultAsync(ct) ?? throw new ModelNotFoundException<Todo>(command.TodoId.Value);

        var attachment = Attachment.Create(
            todo.Id,
            command.Body.FileName,
            command.Body.ContentType
        );

        var sas = attachmentBlobContainerService.GenerateUploadSas(attachment);

        todo.Attachments.Add(attachment);

        context.Update(todo);
        await context.SaveChangesAsync(ct);

        return new Response(
            attachment.Id.Value,
            sas.UploadUrl,
            sas.BlobPath
        );
    }

    public sealed record Response(
        Guid AttachmentId,
        string UploadUrl,
        string BlobPath);

    [Validate]
    public sealed partial record Command : IValidationTarget<Command>
    {
        [FromRoute] [NotEmpty] public required TodoId TodoId { get; init; }
        [FromBody] [NotNull] public required CommandBody Body { get; init; }

        [Validate]
        public sealed partial record CommandBody : IValidationTarget<CommandBody>
        {
            [NotEmpty] public required string FileName { get; init; }
            [NotEmpty] public required string ContentType { get; init; }
        }
    }
}