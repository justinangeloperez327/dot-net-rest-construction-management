using Construction.Application.Common.Messaging;
using Construction.Domain.Companies;

namespace Construction.Application.Companies.UpdateCompany;

public sealed record UpdateCompanyCommand(
    Guid CompanyId,
    string Name,
    CompanyType Type) : ICommand<CompanyResponse>;
