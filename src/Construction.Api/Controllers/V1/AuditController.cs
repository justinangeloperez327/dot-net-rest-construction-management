using Construction.Api.Authorization;
using Construction.Api.Configuration;
using Construction.Api.Extensions;
using Construction.Application.Audit.GetAuditLogs;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Pagination;
using Construction.Domain.Audit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Construction.Api.Controllers.V1;

[ApiController]
[Authorize]
[Route(ApiRoutes.Version1 + "/audit")]
public sealed class AuditController(
    GetAuditLogsQueryHandler auditHandler)
    : ControllerBase
{
    [HttpGet]
    [HasPermission(Permissions.Audit.View)]
    public async Task<IActionResult> ListAsync(
        [FromQuery] Guid? projectId = null,
        [FromQuery] AuditCategory? category = null,
        [FromQuery] Guid? userId = null,
        [FromQuery] DateTimeOffset? fromUtc = null,
        [FromQuery] DateTimeOffset? toUtc = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 25,
        CancellationToken cancellationToken = default)
    {
        var result = await auditHandler.HandleAsync(
            new GetAuditLogsQuery(
                projectId,
                category,
                userId,
                fromUtc,
                toUtc,
                new PageRequest(pageNumber, pageSize)),
            cancellationToken);

        return result.ToActionResult();
    }
}
