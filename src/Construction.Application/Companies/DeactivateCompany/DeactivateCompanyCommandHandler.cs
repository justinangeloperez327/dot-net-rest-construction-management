using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;

namespace Construction.Application.Companies.DeactivateCompany;

public sealed class DeactivateCompanyCommandHandler(
    ICompanyRepository companies,
    IApplicationDbContext dbContext)
    : ICommandHandler<DeactivateCompanyCommand>
{
    public async Task<Result> HandleAsync(
        DeactivateCompanyCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var company = await companies.GetAsync(
            command.CompanyId,
            cancellationToken);

        if (company is null)
        {
            return Result.Failure(
                ApplicationError.NotFound(
                    "Companies.NotFound",
                    "The company was not found."));
        }

        company.Deactivate();
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
