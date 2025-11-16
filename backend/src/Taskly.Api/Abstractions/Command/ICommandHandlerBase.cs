using Mediator;
using Taskly.Core.Functional;

namespace Taskly.Api.Abstractions.Command;

public interface ICommandHandlerBase<in TCommand> : ICommandHandler<TCommand, Result> 
    where TCommand : ICommandBase;
    
public interface ICommandHandlerBase<in TCommand, TResult> : ICommandHandler<TCommand, Result<TResult>> 
    where TCommand : ICommandBase<TResult>;