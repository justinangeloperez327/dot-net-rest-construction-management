using Construction.Api.Authorization;
using Construction.Api.Configuration;
using Construction.Api.Contracts.Suppliers;
using Construction.Api.Extensions;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Pagination;
using Construction.Application.Suppliers.CreateSupplier;
using Construction.Application.Suppliers.GetSupplier;
using Construction.Application.Suppliers.GetSuppliers;
using Construction.Application.Suppliers.UpdateSupplier;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Construction.Api.Controllers.V1;

[ApiController]
[Authorize]
[Route(ApiRoutes.Version1 + "/suppliers")]
public sealed class SuppliersController(
    CreateSupplierCommandHandler createHandler,
    GetSupplierQueryHandler getHandler,
    GetSuppliersQueryHandler listHandler,
    UpdateSupplierCommandHandler updateHandler)
    : ControllerBase
{
    [HttpPost]
    [HasPermission(Permissions.Procurement.ManageSuppliers)]
    public async Task<IActionResult> CreateAsync(
        CreateSupplierRequest request,
        CancellationToken cancellationToken)
    {
        var result = await createHandler.HandleAsync(
            new CreateSupplierCommand(
                request.CompanyId,
                request.Code,
                request.TaxRegistrationNumber,
                request.ContactName,
                request.ContactEmail,
                request.ContactPhone,
                request.PaymentTerms),
            cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(
                nameof(GetAsync),
                new { supplierId = result.Value.Id },
                result.Value)
            : result.ToActionResult();
    }

    [HttpGet("{supplierId:guid}")]
    [HasPermission(Permissions.Procurement.View)]
    public async Task<IActionResult> GetAsync(
        Guid supplierId,
        CancellationToken cancellationToken)
    {
        var result = await getHandler.HandleAsync(
            new GetSupplierQuery(supplierId),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpGet]
    [HasPermission(Permissions.Procurement.View)]
    public async Task<IActionResult> ListAsync(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 25,
        CancellationToken cancellationToken = default)
    {
        var result = await listHandler.HandleAsync(
            new GetSuppliersQuery(new PageRequest(pageNumber, pageSize)),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPut("{supplierId:guid}")]
    [HasPermission(Permissions.Procurement.ManageSuppliers)]
    public async Task<IActionResult> UpdateAsync(
        Guid supplierId,
        UpdateSupplierRequest request,
        CancellationToken cancellationToken)
    {
        var result = await updateHandler.HandleAsync(
            new UpdateSupplierCommand(
                supplierId,
                request.TaxRegistrationNumber,
                request.ContactName,
                request.ContactEmail,
                request.ContactPhone,
                request.PaymentTerms,
                request.Status),
            cancellationToken);

        return result.ToActionResult();
    }
}
