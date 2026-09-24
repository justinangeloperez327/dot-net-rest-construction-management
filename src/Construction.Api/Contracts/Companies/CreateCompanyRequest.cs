using Construction.Domain.Companies;

namespace Construction.Api.Contracts.Companies;

public sealed record CreateCompanyRequest(
    string Name,
    CompanyType Type);
