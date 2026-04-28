namespace Taskly.WebApi.Client.Common;

internal sealed class WebClientService(
    IAttachmentService attachmentService,
    ITagService tagService,
    ITodoService todoService,
    IUserService userService)
    : IWebClientService
{
    public IAttachmentService AttachmentService => attachmentService;
    public ITagService TagService => tagService;
    public ITodoService TodoService => todoService;
    public IUserService UserService => userService;
}