using Api.Client.Attachments;
using Api.Client.Tags;
using Api.Client.Todos;
using Api.Client.Users;

namespace Api.Client;

/// <summary>
///     Contract used by integration tests to perform HTTP requests via Refit.
/// </summary>
public interface IApiClient : 
    IAttachmentEndpoints, 
    ITagEndpoints, 
    ITodoEndpoints, 
    IUserEndpoints;