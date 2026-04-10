using Taskly.WebApi.Client.Abstractions.Endpoints;

namespace Taskly.WebApi.Client.Abstractions;

/// <summary>
///     Contract used by integration tests to perform HTTP requests via Refit.
/// </summary>
public interface IApiClient :
    IAttachmentEndpoints,
    ITagEndpoints,
    ITodoEndpoints,
    IUserEndpoints;