using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Quality;
using Construction.Application.Common.Results;

namespace Construction.Application.Issues.CorrectiveActions;

public sealed class UpdateCorrectiveActionCommandHandler(
    IIssueRepository issues,
    IProjectMemberRepository members,
    IApplicationDbContext dbContext,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService,
    TimeProvider timeProvider)
    : ICommandHandler<UpdateCorrectiveActionCommand, CorrectiveActionResponse>
{
    public async Task<Result<CorrectiveActionResponse>> HandleAsync(
        UpdateCorrectiveActionCommand command,
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
            return Result.Failure<CorrectiveActionResponse>(accessError);
        }

        if (currentUser.UserId is not Guid userId)
        {
            return Result.Failure<CorrectiveActionResponse>(
                ApplicationError.Unauthorized(
                    "Authentication.Required",
                    "Authentication is required."));
        }

        var issue = await issues.GetAsync(command.IssueId, cancellationToken);

        if (issue is null || issue.ProjectId != command.ProjectId)
        {
            return Result.Failure<CorrectiveActionResponse>(
                ApplicationError.NotFound(
                    "Issues.NotFound",
                    "The issue was not found."));
        }

        ApplicationError? responsibleError =
            await QualityReferenceValidator.ValidateProjectMemberAsync(
                command.ProjectId,
                command.ResponsibleUserId,
                members,
                "Issues.InvalidCorrectiveActionResponsibleUser",
                "The corrective action responsible user must be an active project member.",
                cancellationToken);

        if (responsibleError is not null)
        {
            return Result.Failure<CorrectiveActionResponse>(
                responsibleError);
        }

        issue.UpdateCorrectiveAction(
            command.CorrectiveActionId,
            command.Description,
            command.ResponsibleUserId,
            command.DueDate,
            command.Status,
            command.CompletionNotes,
            userId,
            timeProvider.GetUtcNow());

        await dbContext.SaveChangesAsync(cancellationToken);

        var action = issue.CorrectiveActions.Single(
            item => item.Id == command.CorrectiveActionId);

        return Result.Success(
            CorrectiveActionResponse.FromDomain(action));
    }
}
