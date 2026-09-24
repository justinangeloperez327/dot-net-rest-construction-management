using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;

namespace Construction.Application.Companies.GetCompany;

public sealed class GetCompanyQueryHandler(ICompanyRepository companies)
    : IQueryHandler<GetCompanyQuery, CompanyResponse>
{
    public async Task<Result<CompanyResponse>> HandleAsync(
        GetCompanyQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var company = await companies.GetAsync(
            query.CompanyId,
            cancellationToken);

        return company is null
            ? Result.Failure<CompanyResponse>(
                ApplicationError.NotFound(
                    "Companies.NotFound",
                    "The company was not found."))
            : Result.Success(CompanyResponse.FromDomain(company));
    }
}
