using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;
using Construction.Domain.Companies;
using Construction.Domain.Suppliers;

namespace Construction.Application.Suppliers.CreateSupplier;

public sealed class CreateSupplierCommandHandler(
    ICompanyRepository companies,
    ISupplierRepository suppliers,
    IApplicationDbContext dbContext,
    ICurrentUser currentUser)
    : ICommandHandler<CreateSupplierCommand, SupplierResponse>
{
    public async Task<Result<SupplierResponse>> HandleAsync(
        CreateSupplierCommand command,
        CancellationToken cancellationToken = default)
    {
        if (!currentUser.HasPermission(Permissions.Procurement.ManageSuppliers))
        {
            return Result.Failure<SupplierResponse>(
                ApplicationError.Forbidden(
                    "Authorization.PermissionRequired",
                    "Supplier management permission is required."));
        }

        Company? company = await companies.GetAsync(
            command.CompanyId,
            cancellationToken);

        if (company is null
            || company.Type != CompanyType.Supplier
            || company.Status != CompanyStatus.Active)
        {
            return Result.Failure<SupplierResponse>(
                ApplicationError.Validation(
                    "Suppliers.InvalidCompany",
                    "Supplier profile must reference an active Supplier company."));
        }

        if (await suppliers.ExistsByCompanyAsync(
            command.CompanyId,
            cancellationToken))
        {
            return Result.Failure<SupplierResponse>(
                ApplicationError.Conflict(
                    "Suppliers.CompanyAlreadyRegistered",
                    "The company already has a supplier profile."));
        }

        if (await suppliers.ExistsByCodeAsync(
            command.Code.Trim().ToUpperInvariant(),
            cancellationToken))
        {
            return Result.Failure<SupplierResponse>(
                ApplicationError.Conflict(
                    "Suppliers.CodeAlreadyExists",
                    "The supplier code already exists."));
        }

        Supplier supplier = Supplier.Create(
            command.CompanyId,
            command.Code,
            command.TaxRegistrationNumber,
            command.ContactName,
            command.ContactEmail,
            command.ContactPhone,
            command.PaymentTerms);

        suppliers.Add(supplier);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(SupplierResponse.FromDomain(supplier));
    }
}
