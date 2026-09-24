using Construction.Domain.Projects;

namespace Construction.Api.Contracts.Projects;

public sealed record ChangeProjectStatusRequest(
    ProjectStatus Status,
    DateOnly? CompletionDate);
