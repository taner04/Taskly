namespace Taskly.WebApi.Client;

public interface IWebClientService
{
    IAttachmentService AttachmentService { get; }
    ITagService TagService { get; }
    ITodoService TodoService { get; }
    IUserService UserService { get; }
}