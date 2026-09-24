using Construction.Application.Common.Messaging;
using Construction.Domain.Projects;

namespace Construction.Application.Projects.ChangeProjectStatus;

public sealed record ChangeProjectStatusCommand(
    Guid ProjectId,
    ProjectStatus Status,
    DateOnly? CompletionDate = null) : ICommand<ProjectResponse>;
