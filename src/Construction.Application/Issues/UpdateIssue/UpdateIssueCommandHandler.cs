using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Quality;
using Construction.Application.Common.Results;

namespace Construction.Application.Issues.UpdateIssue;

public sealed class UpdateIssueCommandHandler(
    IIssueRepository issues,
    IProjectLocationRepository locations,
    IActivityRepository activities,
    IProjectMemberRepository members,
    IApplicationDbContext dbContext,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService,
    TimeProvider timeProvider)
    : ICommandHandler<UpdateIssueCommand, IssueResponse>
{
    public async Task<Result<IssueResponse>> HandleAsync(
        UpdateIssueCommand command,
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

        var issue = await issues.GetAsync(
            command.IssueId,
            cancellationToken);

        if (issue is null || issue.ProjectId != command.ProjectId)
        {
            return Result.Failure<IssueResponse>(
                ApplicationError.NotFound(
                    "Issues.NotFound",
                    "The issue was not found."));
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

        issue.Update(
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

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(IssueResponse.FromDomain(issue));
    }
}
