using Mediator;
using Taskly.Core.Functional;

namespace Taskly.Api.Abstractions.Command;

public interface ICommandBase : ICommand<Result>;
public interface ICommandBase<T> : ICommand<Result<T>>;