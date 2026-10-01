using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Pagination;
using Construction.Application.Common.Results;

namespace Construction.Application.Audit.GetAuditLogs;

public sealed class GetAuditLogsQueryHandler(
    IAuditRepository audit,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService)
    : IQueryHandler<GetAuditLogsQuery, PagedResult<AuditLogResponse>>
{
    public async Task<Result<PagedResult<AuditLogResponse>>> HandleAsync(
        GetAuditLogsQuery query,
        CancellationToken cancellationToken = default)
    {
        if (!currentUser.Permissions.Contains(Permissions.Audit.View))
        {
            return Result.Failure<PagedResult<AuditLogResponse>>(
                ApplicationError.Forbidden(
                    "Authorization.PermissionRequired",
                    "Audit view permission is required."));
        }

        if (query.ProjectId is Guid projectId)
        {
            ApplicationError? accessError =
                await ProjectAccessGuard.CheckAsync(
                    currentUser,
                    projectAccessService,
                    projectId,
                    Permissions.Audit.View,
                    cancellationToken);

            if (accessError is not null)
            {
                return Result.Failure<PagedResult<AuditLogResponse>>(
                    accessError);
            }
        }
        else if (!currentUser.Permissions.Contains(
            Permissions.Projects.AccessAll))
        {
            return Result.Failure<PagedResult<AuditLogResponse>>(
                ApplicationError.Forbidden(
                    "Audit.GlobalAccessRequired",
                    "Global audit access requires projects.access-all."));
        }

        if (query.FromUtc is DateTimeOffset fromUtc
            && query.ToUtc is DateTimeOffset toUtc
            && fromUtc > toUtc)
        {
            return Result.Failure<PagedResult<AuditLogResponse>>(
                ApplicationError.Validation(
                    "Audit.InvalidDateRange",
                    "Audit start date cannot be after the end date."));
        }

        var page = await audit.GetPageAsync(
            query.ProjectId,
            query.Category,
            query.UserId,
            query.FromUtc,
            query.ToUtc,
            query.Page,
            cancellationToken);

        return Result.Success(
            new PagedResult<AuditLogResponse>(
                page.Items.Select(AuditLogResponse.FromDomain).ToArray(),
                Math.Max(1, query.Page.PageNumber),
                Math.Clamp(
                    query.Page.PageSize,
                    1,
                    PageRequest.MaximumPageSize),
                page.TotalCount));
    }
}
