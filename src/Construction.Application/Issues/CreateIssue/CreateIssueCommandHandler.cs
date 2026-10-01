using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Quality;
using Construction.Application.Common.Results;
using Construction.Domain.Issues;

namespace Construction.Application.Issues.CreateIssue;

public sealed class CreateIssueCommandHandler(
    IProjectRepository projects,
    IProjectLocationRepository locations,
    IActivityRepository activities,
    IProjectMemberRepository members,
    IIssueRepository issues,
    IApplicationDbContext dbContext,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService,
    TimeProvider timeProvider)
    : ICommandHandler<CreateIssueCommand, IssueResponse>
{
    public async Task<Result<IssueResponse>> HandleAsync(
        CreateIssueCommand command,
        CancellationToken cancellationToken = default)
    {
        ApplicationError? accessError =
            await ProjectAccessGuard.CheckAsync(
                currentUser,
                projectAccessService,
                command.ProjectId,
                Permissions.Issues.Manage,
                cancellationToken);

        if (accessError is not null)
        {
            return Result.Failure<IssueResponse>(accessError);
        }

        if (currentUser.UserId is not Guid userId)
        {
            return Result.Failure<IssueResponse>(
                ApplicationError.Unauthorized(
                    "Authentication.Required",
                    "Authentication is required."));
        }

        if (await projects.GetAsync(command.ProjectId, cancellationToken) is null)
        {
            return Result.Failure<IssueResponse>(
                ApplicationError.NotFound(
                    "Projects.NotFound",
                    "The project was not found."));
        }

        ApplicationError? referenceError =
            await QualityReferenceValidator.ValidateLocationAndActivityAsync(
                command.ProjectId,
                command.LocationId,
                command.ActivityId,
                locations,
                activities,
                "Issues",
                cancellationToken);

        if (referenceError is not null)
        {
            return Result.Failure<IssueResponse>(referenceError);
        }

        ApplicationError? responsibleError =
            await QualityReferenceValidator.ValidateProjectMemberAsync(
                command.ProjectId,
                command.ResponsibleUserId,
                members,
                "Issues.InvalidResponsibleUser",
                "The responsible user must be an active project member.",
                cancellationToken);

        if (responsibleError is not null)
        {
            return Result.Failure<IssueResponse>(responsibleError);
        }

        string normalizedNumber =
            command.Number.Trim().ToUpperInvariant();

        if (await issues.ExistsByNumberAsync(
            command.ProjectId,
            normalizedNumber,
            cancellationToken))
        {
            return Result.Failure<IssueResponse>(
                ApplicationError.Conflict(
                    "Issues.NumberAlreadyExists",
                    "An issue with the same number already exists in this project."));
        }

        Issue issue = Issue.Create(
            command.ProjectId,
            command.Number,
            command.Title,
            command.Description,
            command.Type,
            command.Severity,
            command.LocationId,
            command.ActivityId,
            command.ResponsibleUserId,
            command.DueDate,
            userId,
            timeProvider.GetUtcNow());

        issues.Add(issue);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(IssueResponse.FromDomain(issue));
    }
}
