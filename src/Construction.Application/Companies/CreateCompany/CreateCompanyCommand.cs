using Construction.Application.Common.Messaging;
using Construction.Domain.Companies;

namespace Construction.Application.Companies.CreateCompany;

public sealed record CreateCompanyCommand(
    string Name,
    CompanyType Type) : ICommand<CompanyResponse>;
