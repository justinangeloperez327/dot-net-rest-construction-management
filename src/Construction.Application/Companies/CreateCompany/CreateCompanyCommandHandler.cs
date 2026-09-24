using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;
using Construction.Domain.Companies;

namespace Construction.Application.Companies.CreateCompany;

public sealed class CreateCompanyCommandHandler(
    ICompanyRepository companies,
    IApplicationDbContext dbContext)
    : ICommandHandler<CreateCompanyCommand, CompanyResponse>
{
    public async Task<Result<CompanyResponse>> HandleAsync(
        CreateCompanyCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        string normalizedName = command.Name.Trim().ToUpperInvariant();

        if (await companies.ExistsByNameAsync(
            normalizedName,
            cancellationToken: cancellationToken))
        {
            return Result.Failure<CompanyResponse>(
                ApplicationError.Conflict(
                    "Companies.NameAlreadyExists",
                    "A company with the same name already exists."));
        }

        Company company = Company.Create(
            command.Name,
            command.Type);

        companies.Add(company);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(CompanyResponse.FromDomain(company));
    }
}
