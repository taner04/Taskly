using Mediator;
using Taskly.Core.Functional;

namespace Taskly.Api.Abstractions.Query;

public interface IQuerHandlerBase<in TQuery, TResult> : IQueryHandler<TQuery, Result<TResult>>
    where TQuery : IQueryBase<TResult>;