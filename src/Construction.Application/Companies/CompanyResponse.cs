using Construction.Domain.Companies;

namespace Construction.Application.Companies;

public sealed record CompanyResponse(
    Guid Id,
    string Name,
    CompanyType Type,
    CompanyStatus Status,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? LastModifiedAtUtc)
{
    public static CompanyResponse FromDomain(Company company)
    {
        ArgumentNullException.ThrowIfNull(company);

        return new CompanyResponse(
            company.Id,
            company.Name,
            company.Type,
            company.Status,
            company.CreatedAtUtc,
            company.LastModifiedAtUtc);
    }
}
