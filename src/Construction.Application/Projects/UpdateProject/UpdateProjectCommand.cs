using Construction.Application.Common.Messaging;

namespace Construction.Application.Projects.UpdateProject;

public sealed record UpdateProjectCommand(
    Guid ProjectId,
    string Name,
    string? Description,
    DateOnly? StartDate,
    DateOnly? PlannedEndDate,
    Guid? ClientCompanyId,
    Guid? MainContractorCompanyId,
    Guid? ConsultantCompanyId) : ICommand<ProjectResponse>;
