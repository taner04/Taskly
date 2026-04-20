using Taskly.Desktop.Common.Shared.Models;
using Taskly.Shared.Pagination;
using Taskly.WebApi.Client.Common.Shared.Results;

namespace Taskly.Desktop.Common.Abstractions;

public interface IModelService<TModel> where TModel : Model
{
    Task<WebClientResult<PaginationResult<TModel>>> LoadModelsAsync(
        PaginationQuery query,
        CancellationToken cancellationToken = default);
}