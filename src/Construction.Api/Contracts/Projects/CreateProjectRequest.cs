namespace Construction.Api.Contracts.Projects;

public sealed record CreateProjectRequest(
    string Number,
    string Name,
    string? Description,
    DateOnly? StartDate,
    DateOnly? PlannedEndDate,
    Guid? ClientCompanyId,
    Guid? MainContractorCompanyId,
    Guid? ConsultantCompanyId);
