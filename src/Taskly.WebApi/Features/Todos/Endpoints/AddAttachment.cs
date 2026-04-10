using Ardalis.Specification.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using Taskly.WebApi.Common.Infrastructure.Persistence;
using Taskly.WebApi.Common.Shared;
using Taskly.WebApi.Common.Shared.Exceptions;
using Taskly.WebApi.Features.Attachments.Services;
using Taskly.WebApi.Features.Todos.Models;
using Taskly.WebApi.Features.Todos.Specifications;
using TodoId = Taskly.WebApi.Features.Todos.Models.TodoId;

namespace Taskly.WebApi.Features.Todos.Endpoints;

//TODO: Add support for multiple files
[Handler]
[MapPost(ApiRoutes.Todos.AddAttachment)]
[Authorize(Policy = Policies.Roles.User)]
public static partial class AddAttachment
{
    internal static void CustomizeEndpoint(
        IEndpointConventionBuilder endpoint)
    {
        endpoint.WithTags(nameof(Todo));
        endpoint.RequireRateLimiting(Policies.RateLimiting.Global);
    }

    internal static Ok<Response> TransformResult(
        Response response) =>
        TypedResults.Ok(response);


    private static async ValueTask<Response> HandleAsync(
        [AsParameters] Command command,
        TasklyDbContext context,
        CurrentUserService currentUserService,
        AttachmentService attachments,
        CancellationToken ct)
    {
        var userId = currentUserService.GetUserId();

        var spec = new TodoByUserIdSpecificationWithAttachmentsSpec(command.TodoId, userId);
        var todo = await context.Todos
            .WithSpecification(spec)
            .SingleOrDefaultAsync(ct) ?? throw new ModelNotFoundException<Todo>(command.TodoId.Value);

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