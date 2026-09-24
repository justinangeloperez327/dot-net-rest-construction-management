using Construction.Domain.Companies;

namespace Construction.Api.Contracts.Companies;

public sealed record UpdateCompanyRequest(
    string Name,
    CompanyType Type);
