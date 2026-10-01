using Construction.Application.Common.Messaging;
using Construction.Domain.Suppliers;

namespace Construction.Application.Suppliers.UpdateSupplier;

public sealed record UpdateSupplierCommand(
    Guid SupplierId,
    string? TaxRegistrationNumber,
    string? ContactName,
    string? ContactEmail,
    string? ContactPhone,
    string? PaymentTerms,
    SupplierStatus Status) : ICommand<SupplierResponse>;
