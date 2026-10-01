namespace Construction.Api.Contracts.Suppliers;

public sealed record CreateSupplierRequest(
    Guid CompanyId,
    string Code,
    string? TaxRegistrationNumber,
    string? ContactName,
    string? ContactEmail,
    string? ContactPhone,
    string? PaymentTerms);
