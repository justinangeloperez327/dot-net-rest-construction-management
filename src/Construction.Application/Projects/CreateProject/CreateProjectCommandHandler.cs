using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;
using Construction.Domain.ProjectMembers;
using Construction.Domain.Projects;

namespace Construction.Application.Projects.CreateProject;

public sealed class CreateProjectCommandHandler(
    IProjectRepository projects,
    IProjectMemberRepository members,
    ICompanyRepository companies,
    IApplicationDbContext dbContext,
    ICurrentUser currentUser)
    : ICommandHandler<CreateProjectCommand, ProjectResponse>
{
    public async Task<Result<ProjectResponse>> HandleAsync(
        CreateProjectCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (currentUser.UserId is not Guid userId)
        {
            return Result.Failure<ProjectResponse>(
                ApplicationError.Unauthorized(
                    "Authentication.Required",
                    "Authentication is required."));
        }

        string normalizedNumber =
            command.Number.Trim().ToUpperInvariant();

        if (await projects.ExistsByNumberAsync(
            normalizedNumber,
            cancellationToken))
        {
            return Result.Failure<ProjectResponse>(
                ApplicationError.Conflict(
                    "Projects.NumberAlreadyExists",
                    "A project with the same number already exists."));
        }

        ApplicationError? companyError =
            await ValidateCompaniesAsync(command, cancellationToken);

        if (companyError is not null)
        {
            return Result.Failure<ProjectResponse>(companyError);
        }

        Project project = Project.Create(
            command.Number,
            command.Name,
            command.Description,
            command.StartDate,
            command.PlannedEndDate,
            command.ClientCompanyId,
            command.MainContractorCompanyId,
            command.ConsultantCompanyId);

        projects.Add(project);

        members.Add(ProjectMember.Create(
            project.Id,
            userId,
            ProjectMemberRole.ProjectAdministrator));

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(ProjectResponse.FromDomain(project));
    }

    private async Task<ApplicationError?> ValidateCompaniesAsync(
        CreateProjectCommand command,
        CancellationToken cancellationToken)
    {
        Guid?[] ids =
        [
            command.ClientCompanyId,
            command.MainContractorCompanyId,
            command.ConsultantCompanyId
        ];

        foreach (Guid companyId in ids.Where(id => id.HasValue).Select(id => id!.Value))
        {
            if (await companies.GetAsync(companyId, cancellationToken) is null)
            {
                return ApplicationError.NotFound(
                    "Projects.CompanyNotFound",
                    $"Referenced company '{companyId}' was not found.");
            }
        }

        return null;
    }
}
