using System.Text.Json;

namespace Taskly.WebApi.Features.Attachments.WebHooks;

internal static class AttachmentWebHookParser
{
    internal static AttachmentEventData? ParseEventData(string eventType, JsonElement eventDataElement)
    {
        try
        {
            return eventType switch
            {
                AttachmentWebHookConstants.Event.UploadCompleted => ParseCompletedEvent(eventDataElement),
                AttachmentWebHookConstants.Event.UploadFailed => new AttachmentEventData.Failed(),
                _ => null
            };
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static AttachmentEventData.Completed ParseCompletedEvent(JsonElement element)
    {
        if (element.TryGetProperty("fileSize", out var fileSizeElement) &&
            fileSizeElement.TryGetInt64(out var fileSize))
        {
            return new AttachmentEventData.Completed(fileSize);
        }

        throw new JsonException("EventData must have 'fileSize' property of type 'long'");
    }
}