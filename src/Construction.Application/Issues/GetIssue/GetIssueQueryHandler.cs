using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;

namespace Construction.Application.Issues.GetIssue;

public sealed class GetIssueQueryHandler(
    IIssueRepository issues,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService)
    : IQueryHandler<GetIssueQuery, IssueResponse>
{
    public async Task<Result<IssueResponse>> HandleAsync(
        GetIssueQuery query,
        CancellationToken cancellationToken = default)
    {
        ApplicationError? accessError =
            await ProjectAccessGuard.CheckAsync(
                currentUser,
                projectAccessService,
                query.ProjectId,
                Permissions.Issues.View,
                cancellationToken);

        if (accessError is not null)
        {
            return Result.Failure<IssueResponse>(accessError);
        }

        var issue = await issues.GetAsync(
            query.IssueId,
            cancellationToken);

        return issue is null || issue.ProjectId != query.ProjectId
            ? Result.Failure<IssueResponse>(
                ApplicationError.NotFound(
                    "Issues.NotFound",
                    "The issue was not found."))
            : Result.Success(IssueResponse.FromDomain(issue));
    }
}
