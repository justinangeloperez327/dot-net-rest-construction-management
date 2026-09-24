using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;

namespace Construction.Application.Companies.UpdateCompany;

public sealed class UpdateCompanyCommandHandler(
    ICompanyRepository companies,
    IApplicationDbContext dbContext)
    : ICommandHandler<UpdateCompanyCommand, CompanyResponse>
{
    public async Task<Result<CompanyResponse>> HandleAsync(
        UpdateCompanyCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var company = await companies.GetAsync(
            command.CompanyId,
            cancellationToken);

        if (company is null)
        {
            return Result.Failure<CompanyResponse>(
                ApplicationError.NotFound(
                    "Companies.NotFound",
                    "The company was not found."));
        }

        string normalizedName = command.Name.Trim().ToUpperInvariant();

        if (await companies.ExistsByNameAsync(
            normalizedName,
            command.CompanyId,
            cancellationToken))
        {
            return Result.Failure<CompanyResponse>(
                ApplicationError.Conflict(
                    "Companies.NameAlreadyExists",
                    "A company with the same name already exists."));
        }

        company.Update(command.Name, command.Type);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(CompanyResponse.FromDomain(company));
    }
}
