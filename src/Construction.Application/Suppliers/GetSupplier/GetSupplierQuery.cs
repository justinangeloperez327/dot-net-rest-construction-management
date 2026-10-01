using Construction.Application.Common.Messaging;

namespace Construction.Application.Suppliers.GetSupplier;

public sealed record GetSupplierQuery(Guid SupplierId) : IQuery<SupplierResponse>;
