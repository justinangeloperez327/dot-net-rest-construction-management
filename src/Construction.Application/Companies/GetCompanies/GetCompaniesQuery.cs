using Construction.Application.Common.Messaging;
using Construction.Application.Common.Pagination;

namespace Construction.Application.Companies.GetCompanies;

public sealed record GetCompaniesQuery(PageRequest Page)
    : IQuery<PagedResult<CompanyResponse>>;
