using Taskly.WebApi.Features.Todos.Common.Extensions;
using Taskly.WebApi.Features.Todos.Common.Specifications;
using AttachmentBlobContainerService =
    Taskly.WebApi.Features.Attachments.Common.Services.AttachmentBlobContainerService;

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

    internal static Ok<AddAttachmentResponse> TransformResult(
        AddAttachmentResponse response) =>
        TypedResults.Ok(response);

    private static async ValueTask<AddAttachmentResponse> HandleAsync(
        [AsParameters] Command command,
        TasklyDbContext context,
        CurrentUserService currentUserService,
        AttachmentBlobContainerService attachmentBlobContainerService,
        CancellationToken ct)
    {
        var userId = currentUserService.GetUserId();

        var spec = new TodoByUserIdSpecification(command.TodoId, userId);
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

        return new AddAttachmentResponse(todo.ToGetTodoResponse(), sas.UploadUrl);
    }

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