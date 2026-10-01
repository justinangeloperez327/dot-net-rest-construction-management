using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;
using Construction.Domain.Suppliers;

namespace Construction.Application.Suppliers.UpdateSupplier;

public sealed class UpdateSupplierCommandHandler(
    ISupplierRepository suppliers,
    IApplicationDbContext dbContext,
    ICurrentUser currentUser)
    : ICommandHandler<UpdateSupplierCommand, SupplierResponse>
{
    public async Task<Result<SupplierResponse>> HandleAsync(
        UpdateSupplierCommand command,
        CancellationToken cancellationToken = default)
    {
        if (!currentUser.Permissions.Contains(Permissions.Procurement.ManageSuppliers))
        {
            return Result.Failure<SupplierResponse>(
                ApplicationError.Forbidden(
                    "Authorization.PermissionRequired",
                    "Supplier management permission is required."));
        }

        Supplier? supplier = await suppliers.GetAsync(command.SupplierId, cancellationToken);

        if (supplier is null)
        {
            return Result.Failure<SupplierResponse>(
                ApplicationError.NotFound("Suppliers.NotFound", "The supplier was not found."));
        }

        if (command.Status == SupplierStatus.Active
            && supplier.Status == SupplierStatus.Inactive)
        {
            supplier.Activate();
        }

        if (command.Status == SupplierStatus.Inactive)
        {
            supplier.Deactivate();
        }
        else
        {
            supplier.Update(
                command.TaxRegistrationNumber,
                command.ContactName,
                command.ContactEmail,
                command.ContactPhone,
                command.PaymentTerms);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(SupplierResponse.FromDomain(supplier));
    }
}
