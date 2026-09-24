using Construction.Application.Common.Messaging;

namespace Construction.Application.Companies.GetCompany;

public sealed record GetCompanyQuery(Guid CompanyId)
    : IQuery<CompanyResponse>;
