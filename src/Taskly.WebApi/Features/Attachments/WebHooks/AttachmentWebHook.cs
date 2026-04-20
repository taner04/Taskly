using System.Text.Json;
using Taskly.WebApi.Common.Abstractions;
using Taskly.WebApi.Features.Attachments.WebHooks.EventStrategies;

namespace Taskly.WebApi.Features.Attachments.WebHooks;

[Handler]
[MapPost(ApiRoutes.Attachments.WebHook)]
public static partial class AttachmentEventWebHook
{
    internal static void CustomizeEndpoint(
        RouteHandlerBuilder endpoint)
    {
        endpoint.WithTags(nameof(Attachment));
        endpoint.AllowAnonymous();
        endpoint.RequireRateLimiting(Security.RateLimiting.Global);
        endpoint.AddEndpointFilter<WebHookSecretEndpointFilter>();
    }

    private static async ValueTask HandleAsync(
        Command command,
        ILoggerFactory loggerFactory,
        TasklyDbContext context,
        AttachmentEventDispatcher dispatcher,
        CancellationToken ct)
    {
        var logger = loggerFactory.CreateLogger(nameof(AttachmentEventWebHook));
        var eventData = AttachmentWebHookParser.ParseEventData(command.Body.EventType, command.Body.EventData);

        if (eventData is null)
        {
            logger.LogWarning("Could not parse event data for event type: {EventType}", command.Body.EventType);
            return;
        }

        var attachment =
            await context.Attachments.FirstOrDefaultAsync(a => a.Id == AttachmentId.From(command.Body.AttachmentId),
                ct) ?? throw new ModelNotFoundException<AttachmentEventData>(command.Body.AttachmentId);

        await dispatcher.HandleAsync(attachment, eventData, ct);
    }

    [Validate]
    public sealed partial record Command : IValidationTarget<Command>, IWebHookRequest
    {
        [NotNull] public required CommandBody Body { get; init; }

        [Validate]
        public sealed partial record CommandBody : IValidationTarget<CommandBody>
        {
            public required Guid AttachmentId { get; init; }
            public required string EventType { get; init; }
            public required JsonElement EventData { get; init; }
        }
    }
}