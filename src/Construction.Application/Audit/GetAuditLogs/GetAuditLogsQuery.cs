using Construction.Application.Common.Messaging;
using Construction.Application.Common.Pagination;
using Construction.Domain.Audit;

namespace Construction.Application.Audit.GetAuditLogs;

public sealed record GetAuditLogsQuery(
    Guid? ProjectId,
    AuditCategory? Category,
    Guid? UserId,
    DateTimeOffset? FromUtc,
    DateTimeOffset? ToUtc,
    PageRequest Page)
    : IQuery<PagedResult<AuditLogResponse>>;
