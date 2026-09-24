using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Pagination;
using Construction.Application.Common.Results;

namespace Construction.Application.Companies.GetCompanies;

public sealed class GetCompaniesQueryHandler(ICompanyRepository companies)
    : IQueryHandler<GetCompaniesQuery, PagedResult<CompanyResponse>>
{
    public async Task<Result<PagedResult<CompanyResponse>>> HandleAsync(
        GetCompaniesQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var page = await companies.GetPageAsync(
            query.Page,
            cancellationToken);

        var response = new PagedResult<CompanyResponse>(
            page.Items.Select(CompanyResponse.FromDomain).ToArray(),
            Math.Max(1, query.Page.PageNumber),
            Math.Clamp(
                query.Page.PageSize,
                1,
                PageRequest.MaximumPageSize),
            page.TotalCount);

        return Result.Success(response);
    }
}
