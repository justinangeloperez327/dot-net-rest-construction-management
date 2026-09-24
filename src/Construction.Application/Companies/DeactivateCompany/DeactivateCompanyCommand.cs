using Construction.Application.Common.Messaging;

namespace Construction.Application.Companies.DeactivateCompany;

public sealed record DeactivateCompanyCommand(Guid CompanyId)
    : ICommand;
