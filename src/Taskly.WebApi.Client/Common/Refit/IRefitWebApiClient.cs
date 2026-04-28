using Taskly.WebApi.Client.Common.Refit.Endpoints;

namespace Taskly.WebApi.Client.Common.Refit;

public interface IRefitWebApiClient :
    IAttachmentEndpoints,
    ITagEndpoints,
    ITodoEndpoints,
    IUserEndpoints;