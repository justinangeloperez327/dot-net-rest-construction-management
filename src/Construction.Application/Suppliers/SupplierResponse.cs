using Construction.Domain.Suppliers;

namespace Construction.Application.Suppliers;

public sealed record SupplierResponse(
    Guid Id,
    Guid CompanyId,
    string Code,
    SupplierStatus Status,
    string? TaxRegistrationNumber,
    string? ContactName,
    string? ContactEmail,
    string? ContactPhone,
    string? PaymentTerms,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? LastModifiedAtUtc)
{
    public static SupplierResponse FromDomain(Supplier supplier) =>
        new(
            supplier.Id,
            supplier.CompanyId,
            supplier.Code,
            supplier.Status,
            supplier.TaxRegistrationNumber,
            supplier.ContactName,
            supplier.ContactEmail,
            supplier.ContactPhone,
            supplier.PaymentTerms,
            supplier.CreatedAtUtc,
            supplier.LastModifiedAtUtc);
}
