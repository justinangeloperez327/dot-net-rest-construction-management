using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;

namespace Construction.Application.Suppliers.GetSupplier;

public sealed class GetSupplierQueryHandler(
    ISupplierRepository suppliers,
    ICurrentUser currentUser)
    : IQueryHandler<GetSupplierQuery, SupplierResponse>
{
    public async Task<Result<SupplierResponse>> HandleAsync(
        GetSupplierQuery query,
        CancellationToken cancellationToken = default)
    {
        if (!currentUser.HasPermission(Permissions.Procurement.View))
        {
            return Result.Failure<SupplierResponse>(
                ApplicationError.Forbidden(
                    "Authorization.PermissionRequired",
                    "Procurement view permission is required."));
        }

        var supplier = await suppliers.GetAsync(query.SupplierId, cancellationToken);

        return supplier is null
            ? Result.Failure<SupplierResponse>(
                ApplicationError.NotFound("Suppliers.NotFound", "The supplier was not found."))
            : Result.Success(SupplierResponse.FromDomain(supplier));
    }
}
