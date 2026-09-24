namespace Construction.Api.Contracts.Projects;

public sealed record UpdateProjectRequest(
    string Name,
    string? Description,
    DateOnly? StartDate,
    DateOnly? PlannedEndDate,
    Guid? ClientCompanyId,
    Guid? MainContractorCompanyId,
    Guid? ConsultantCompanyId);
