using Mediator;
using Taskly.Core.BuildingBlocks.Domain;

namespace Taskly.Api.Abstractions.DomainEvent;

public interface IDomainEventHandler<in TEvent> 
    : INotificationHandler<TEvent> where TEvent : IDomainEvent;