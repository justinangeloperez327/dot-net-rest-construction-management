using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;

namespace Construction.Application.Projects.UpdateProject;

public sealed class UpdateProjectCommandHandler(
    IProjectRepository projects,
    ICompanyRepository companies,
    IApplicationDbContext dbContext,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService)
    : ICommandHandler<UpdateProjectCommand, ProjectResponse>
{
    public async Task<Result<ProjectResponse>> HandleAsync(
        UpdateProjectCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        ApplicationError? accessError =
            await ProjectAccessGuard.CheckAsync(
                currentUser,
                projectAccessService,
                command.ProjectId,
                Permissions.Projects.Update,
                cancellationToken);

        if (accessError is not null)
        {
            return Result.Failure<ProjectResponse>(accessError);
        }

        var project = await projects.GetAsync(
            command.ProjectId,
            cancellationToken);

        if (project is null)
        {
            return Result.Failure<ProjectResponse>(
                ApplicationError.NotFound(
                    "Projects.NotFound",
                    "The project was not found."));
        }

        foreach (Guid companyId in new Guid?[]
        {
            command.ClientCompanyId,
            command.MainContractorCompanyId,
            command.ConsultantCompanyId
        }.Where(id => id.HasValue).Select(id => id!.Value))
        {
            if (await companies.GetAsync(companyId, cancellationToken) is null)
            {
                return Result.Failure<ProjectResponse>(
                    ApplicationError.NotFound(
                        "Projects.CompanyNotFound",
                        $"Referenced company '{companyId}' was not found."));
            }
        }

        project.Update(
            command.Name,
            command.Description,
            command.StartDate,
            command.PlannedEndDate,
            command.ClientCompanyId,
            command.MainContractorCompanyId,
            command.ConsultantCompanyId);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(ProjectResponse.FromDomain(project));
    }
}
