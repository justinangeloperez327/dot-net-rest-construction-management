using Construction.Domain.Suppliers;

namespace Construction.Api.Contracts.Suppliers;

public sealed record UpdateSupplierRequest(
    string? TaxRegistrationNumber,
    string? ContactName,
    string? ContactEmail,
    string? ContactPhone,
    string? PaymentTerms,
    SupplierStatus Status);
