using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;

namespace Construction.Application.Issues.CancelIssue;

public sealed class CancelIssueCommandHandler(
    IIssueRepository issues,
    IApplicationDbContext dbContext,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService,
    TimeProvider timeProvider)
    : ICommandHandler<CancelIssueCommand, IssueResponse>
{
    public async Task<Result<IssueResponse>> HandleAsync(
        CancelIssueCommand command,
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

        var issue = await issues.GetAsync(command.IssueId, cancellationToken);

        if (issue is null || issue.ProjectId != command.ProjectId)
        {
            return Result.Failure<IssueResponse>(
                ApplicationError.NotFound(
                    "Issues.NotFound",
                    "The issue was not found."));
        }

        issue.Cancel(
            userId,
            timeProvider.GetUtcNow(),
            command.Reason);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(IssueResponse.FromDomain(issue));
    }
}
