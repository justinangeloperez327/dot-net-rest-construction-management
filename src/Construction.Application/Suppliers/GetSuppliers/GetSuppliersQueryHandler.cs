using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Pagination;
using Construction.Application.Common.Results;

namespace Construction.Application.Suppliers.GetSuppliers;

public sealed class GetSuppliersQueryHandler(
    ISupplierRepository suppliers,
    ICurrentUser currentUser)
    : IQueryHandler<GetSuppliersQuery, PagedResult<SupplierResponse>>
{
    public async Task<Result<PagedResult<SupplierResponse>>> HandleAsync(
        GetSuppliersQuery query,
        CancellationToken cancellationToken = default)
    {
        if (!currentUser.Permissions.Contains(Permissions.Procurement.View))
        {
            return Result.Failure<PagedResult<SupplierResponse>>(
                ApplicationError.Forbidden(
                    "Authorization.PermissionRequired",
                    "Procurement view permission is required."));
        }

        var page = await suppliers.GetPageAsync(query.Page, cancellationToken);

        return Result.Success(new PagedResult<SupplierResponse>(
            page.Items.Select(SupplierResponse.FromDomain).ToArray(),
            Math.Max(1, query.Page.PageNumber),
            Math.Clamp(query.Page.PageSize, 1, PageRequest.MaximumPageSize),
            page.TotalCount));
    }
}
