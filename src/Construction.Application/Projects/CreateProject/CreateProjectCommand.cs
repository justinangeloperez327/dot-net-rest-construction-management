using Construction.Application.Common.Messaging;

namespace Construction.Application.Projects.CreateProject;

public sealed record CreateProjectCommand(
    string Number,
    string Name,
    string? Description,
    DateOnly? StartDate,
    DateOnly? PlannedEndDate,
    Guid? ClientCompanyId,
    Guid? MainContractorCompanyId,
    Guid? ConsultantCompanyId) : ICommand<ProjectResponse>;
