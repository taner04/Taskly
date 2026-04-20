using Taskly.WebApi.Client.Common.Services;
using Taskly.WebApi.Client.Features.Attachments.Services;
using Taskly.WebApi.Client.Features.Tags.Services;
using Taskly.WebApi.Client.Features.Todos.Services;
using Taskly.WebApi.Client.Features.Users.Services;

namespace Taskly.WebApi.Client.Common;

public sealed class WebClientService(WebClientOptions options,
    AttachmentService attachmentService,
    TagService tagService,
    TodoService todoService,
    UserService userService,
    BearerTokeStore bearerTokeStore)
{
    public AttachmentService AttachmentService => attachmentService;
    public TagService TagService => tagService;
    public TodoService TodoService => todoService;
    public UserService UserService => userService;

    public void SetAccessToken(string accessToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(accessToken);
        bearerTokeStore.SetAccessToken(accessToken);
    }

    public void ClearAccessToken()
    {
        bearerTokeStore.ClearToken(string.Empty);
    }
}