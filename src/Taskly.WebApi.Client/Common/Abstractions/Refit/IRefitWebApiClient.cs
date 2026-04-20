using Taskly.WebApi.Client.Common.Abstractions.Refit.Endpoints;

namespace Taskly.WebApi.Client.Common.Abstractions.Refit;

public interface IRefitWebApiClient :
    IAttachmentEndpoints,
    ITagEndpoints,
    ITodoEndpoints,
    IUserEndpoints;