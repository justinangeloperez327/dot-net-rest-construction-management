using Construction.Application.Common.Messaging;
using Construction.Application.Common.Pagination;

namespace Construction.Application.Suppliers.GetSuppliers;

public sealed record GetSuppliersQuery(PageRequest Page)
    : IQuery<PagedResult<SupplierResponse>>;
