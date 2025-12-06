using Api.Features.Attachments.Services;
using Api.Features.Todos.Exceptions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.Features.Todos.Endpoints;

[Handler]
[MapPost(Routes.Todos.AddAttachment)]
[Authorize]
public static partial class AddAttachment
{
    internal static void CustomizeEndpoint(
        IEndpointConventionBuilder endpoint)
    {
        endpoint.WithTags(nameof(Todo));
    }

    internal static Ok<Response> TransformResult(
        Response response)
    {
        return TypedResults.Ok(response);
    }


    private static async ValueTask<Response> HandleAsync(
        [AsParameters] Command command,
        ApplicationDbContext context,
        CurrentUserService current,
        AttachmentService attachments,
        CancellationToken ct)
    {
        var userId = current.GetCurrentUserId();
        var todo = await context.Todos
            .Include(t => t.Attachments)
            .SingleOrDefaultAsync(t => t.Id == command.TodoId && t.UserId == userId, ct);

        if (todo is null) throw new TodoNotFoundException(command.TodoId);

        var attachment = Attachment.CreatePending(
            todo.Id,
            command.Body.FileName,
            command.Body.ContentType
        );

        var sas = attachments.GenerateUploadSas(attachment);

        todo.Attachments.Add(attachment);

        context.Update(todo);
        await context.SaveChangesAsync(ct);

        return new Response(
            attachment.Id.Value,
            sas.UploadUrl,
            sas.BlobPath
        );
    }

    [Validate]
    public sealed partial record Command : IValidationTarget<Command>
    {
        [NotEmpty] [FromRoute] public required TodoId TodoId { get; init; }
        [NotNull] public required CommandBody Body { get; init; }

        [Validate]
        public sealed partial record CommandBody : IValidationTarget<CommandBody>
        {
            [NotEmpty] public required string FileName { get; init; }

            [NotEmpty] public required string ContentType { get; init; }
        }
    }

    public sealed record Response(
        Guid AttachmentId,
        string UploadUrl,
        string BlobPath);
}