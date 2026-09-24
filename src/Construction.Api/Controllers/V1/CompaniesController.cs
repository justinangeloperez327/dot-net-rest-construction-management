using Construction.Api.Authorization;
using Construction.Api.Contracts.Companies;
using Construction.Api.Extensions;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Pagination;
using Construction.Application.Companies.CreateCompany;
using Construction.Application.Companies.DeactivateCompany;
using Construction.Application.Companies.GetCompanies;
using Construction.Application.Companies.GetCompany;
using Construction.Application.Companies.UpdateCompany;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Construction.Api.Controllers.V1;

[Authorize]
public sealed class CompaniesController(
    CreateCompanyCommandHandler createHandler,
    GetCompanyQueryHandler getHandler,
    GetCompaniesQueryHandler listHandler,
    UpdateCompanyCommandHandler updateHandler,
    DeactivateCompanyCommandHandler deactivateHandler)
    : ApiControllerBase
{
    [HttpPost]
    [HasPermission(Permissions.Companies.Manage)]
    public async Task<IActionResult> CreateAsync(
        CreateCompanyRequest request,
        CancellationToken cancellationToken)
    {
        var result = await createHandler.HandleAsync(
            new CreateCompanyCommand(request.Name, request.Type),
            cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(
                nameof(GetAsync),
                new { companyId = result.Value.Id },
                result.Value)
            : result.ToActionResult();
    }

    [HttpGet("{companyId:guid}")]
    [HasPermission(Permissions.Companies.View)]
    public async Task<IActionResult> GetAsync(
        Guid companyId,
        CancellationToken cancellationToken)
    {
        var result = await getHandler.HandleAsync(
            new GetCompanyQuery(companyId),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpGet]
    [HasPermission(Permissions.Companies.View)]
    public async Task<IActionResult> ListAsync(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 25,
        CancellationToken cancellationToken = default)
    {
        var result = await listHandler.HandleAsync(
            new GetCompaniesQuery(
                new PageRequest(pageNumber, pageSize)),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPut("{companyId:guid}")]
    [HasPermission(Permissions.Companies.Manage)]
    public async Task<IActionResult> UpdateAsync(
        Guid companyId,
        UpdateCompanyRequest request,
        CancellationToken cancellationToken)
    {
        var result = await updateHandler.HandleAsync(
            new UpdateCompanyCommand(
                companyId,
                request.Name,
                request.Type),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpDelete("{companyId:guid}")]
    [HasPermission(Permissions.Companies.Manage)]
    public async Task<IActionResult> DeactivateAsync(
        Guid companyId,
        CancellationToken cancellationToken)
    {
        var result = await deactivateHandler.HandleAsync(
            new DeactivateCompanyCommand(companyId),
            cancellationToken);

        return result.ToActionResult();
    }
}
