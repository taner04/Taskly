using Microsoft.Extensions.Options;
using Taskly.Shared.Options;

namespace Taskly.WebApi.Features.Attachments.WebHooks;

internal sealed class WebHookSecretEndpointFilter(
    IOptions<WebHookConfig> options) : IEndpointFilter
{
    private readonly WebHookConfig _config = options.Value ?? throw new ArgumentNullException(nameof(options));

    public ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var httpContext = context.HttpContext;

        if (string.IsNullOrEmpty(_config.SecretKey) ||
            !httpContext.Request.Headers.TryGetValue(AttachmentWebHookConstants.RequestHeader, out var webhookSecret) ||
            _config.SecretKey != webhookSecret)
        {
            return new ValueTask<object?>(Results.Unauthorized());
        }

        return next(context);
    }
}