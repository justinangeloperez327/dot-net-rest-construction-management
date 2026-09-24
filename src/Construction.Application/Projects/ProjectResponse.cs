using Construction.Domain.Projects;

namespace Construction.Application.Projects;

public sealed record ProjectResponse(
    Guid Id,
    string Number,
    string Name,
    string? Description,
    ProjectStatus Status,
    DateOnly? StartDate,
    DateOnly? PlannedEndDate,
    DateOnly? ActualEndDate,
    Guid? ClientCompanyId,
    Guid? MainContractorCompanyId,
    Guid? ConsultantCompanyId,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? LastModifiedAtUtc)
{
    public static ProjectResponse FromDomain(Project project)
    {
        ArgumentNullException.ThrowIfNull(project);

        return new ProjectResponse(
            project.Id,
            project.Number,
            project.Name,
            project.Description,
            project.Status,
            project.StartDate,
            project.PlannedEndDate,
            project.ActualEndDate,
            project.ClientCompanyId,
            project.MainContractorCompanyId,
            project.ConsultantCompanyId,
            project.CreatedAtUtc,
            project.LastModifiedAtUtc);
    }
}
