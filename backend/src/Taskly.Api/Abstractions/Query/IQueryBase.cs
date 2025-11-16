using Mediator;
using Taskly.Core.Functional;

namespace Taskly.Api.Abstractions.Query;

public interface IQueryBase<TResult> : IQuery<Result<TResult>>;