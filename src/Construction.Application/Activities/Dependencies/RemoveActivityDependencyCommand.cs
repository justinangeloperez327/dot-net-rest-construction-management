using Construction.Application.Common.Messaging;

namespace Construction.Application.Activities.Dependencies;

public sealed record RemoveActivityDependencyCommand(
    Guid ProjectId,
    Guid ActivityId,
    Guid DependencyId) : ICommand;
