using Api.Features.Attachments.Models;
using Api.Features.Attachments.Services;
using Api.Features.Todos.Model;
using Api.Features.Users;
using Api.Shared.Features.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.Todos;

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

    internal static Results<Ok, NotFound<Error>, BadRequest<Error>, StatusCodeHttpResult>
        TransformResult(
            ErrorOr<Success> result)
    {
        return result.Match<
            Results<Ok, NotFound<Error>, BadRequest<Error>, StatusCodeHttpResult>>
        (
            _ => TypedResults.Ok(),
            errors =>
            {
                var e = errors.First();

                return e.Type switch
                {
                    ErrorType.Validation => TypedResults.BadRequest(e),
                    ErrorType.NotFound => TypedResults.NotFound(e),
                    _ => TypedResults.StatusCode(500)
                };
            }
        );
    }


    private static async ValueTask<ErrorOr<Success>> HandleAsync(
        [AsParameters] Command command,
        ApplicationDbContext db,
        CurrentUserService current,
        AttachmentService attachments,
        CancellationToken ct)
    {
        var userId = current.GetCurrentUserId();

        var todo = await db.Todos
            .Include(t => t.Attachments)
            .SingleOrDefaultAsync(t => t.Id == command.TodoId && t.UserId == userId, ct);

        if (todo is null)
        {
            return Error.NotFound("Todo.NotFound",
                $"The todo does not exist with the specified id '{command.TodoId}'.");
        }

        var file = command.Body.File;

        var createResult = Attachment.TryCreate(todo.Id, file);
        if (createResult.IsError)
        {
            return createResult.Errors;
        }

        var attachment = createResult.Value;

        var uploaded = await attachments.UploadAsync(file, attachment, ct);

        if (!uploaded)
        {
            return Error.Failure("Attachment.UploadFailed", "Blob upload failed.");
        }

        todo.Attachments.Add(attachment);

        return Result.Success;
    }

    [Validate]
    public sealed partial record Command : ITransactionalRequest, IValidationTarget<Command>
    {
        [NotEmpty] [FromRoute] public required TodoId TodoId { get; init; }
        [FromForm] public required CommandBody Body { get; init; }

        [Validate]
        public sealed partial record CommandBody : IValidationTarget<CommandBody>
        {
           [NotEmpty] public required IFormFile File { get; init; }
        }
    }
}