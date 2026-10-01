using Construction.Application.Common.Messaging;

namespace Construction.Application.Suppliers.CreateSupplier;

public sealed record CreateSupplierCommand(
    Guid CompanyId,
    string Code,
    string? TaxRegistrationNumber,
    string? ContactName,
    string? ContactEmail,
    string? ContactPhone,
    string? PaymentTerms) : ICommand<SupplierResponse>;
