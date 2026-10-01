using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Pagination;
using Construction.Application.Common.Results;

namespace Construction.Application.Equipment.GetEquipmentList;

public sealed class GetEquipmentListQueryHandler(
    IEquipmentRepository equipment,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService)
    : IQueryHandler<GetEquipmentListQuery, PagedResult<EquipmentSummaryResponse>>
{
    public async Task<Result<PagedResult<EquipmentSummaryResponse>>> HandleAsync(
        GetEquipmentListQuery query,
        CancellationToken cancellationToken = default)
    {
        ApplicationError? accessError = await ProjectAccessGuard.CheckAsync(
            currentUser,
            projectAccessService,
            query.ProjectId,
            Permissions.Equipment.View,
            cancellationToken);

        if (accessError is not null)
        {
            return Result.Failure<PagedResult<EquipmentSummaryResponse>>(accessError);
        }

        var page = await equipment.GetPageAsync(
            query.ProjectId,
            query.Page,
            cancellationToken);

        return Result.Success(
            new PagedResult<EquipmentSummaryResponse>(
                page.Items.Select(EquipmentSummaryResponse.FromDomain).ToArray(),
                Math.Max(1, query.Page.PageNumber),
                Math.Clamp(query.Page.PageSize, 1, PageRequest.MaximumPageSize),
                page.TotalCount));
    }
}
