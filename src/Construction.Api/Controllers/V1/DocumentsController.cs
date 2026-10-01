using Construction.Api.Authorization;
using Construction.Api.Configuration;
using Construction.Api.Contracts.Documents;
using Construction.Api.Extensions;
using Construction.Application.Abstractions.Files;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Pagination;
using Construction.Application.Documents.AddDocumentRevision;
using Construction.Application.Documents.ArchiveDocument;
using Construction.Application.Documents.CreateDocument;
using Construction.Application.Documents.DownloadDocumentRevision;
using Construction.Application.Documents.GetDocument;
using Construction.Application.Documents.GetDocuments;
using Construction.Application.Documents.UpdateDocument;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Construction.Api.Controllers.V1;

[ApiController]
[Authorize]
[Route(ApiRoutes.Version1 + "/projects/{projectId:guid}/documents")]
public sealed class DocumentsController(
    CreateDocumentCommandHandler createHandler,
    GetDocumentQueryHandler getHandler,
    GetDocumentsQueryHandler listHandler,
    UpdateDocumentCommandHandler updateHandler,
    ArchiveDocumentCommandHandler archiveHandler,
    AddDocumentRevisionCommandHandler addRevisionHandler,
    DownloadDocumentRevisionQueryHandler downloadRevisionHandler)
    : ControllerBase
{
    [HttpPost]
    [HasPermission(Permissions.Documents.Manage)]
    public async Task<IActionResult> CreateAsync(
        Guid projectId,
        CreateDocumentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await createHandler.HandleAsync(
            new CreateDocumentCommand(
                projectId,
                request.Number,
                request.Title,
                request.Category,
                request.Description),
            cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(
                nameof(GetAsync),
                new
                {
                    projectId,
                    documentId = result.Value.Id
                },
                result.Value)
            : result.ToActionResult();
    }

    [HttpGet("{documentId:guid}")]
    [HasPermission(Permissions.Documents.View)]
    public async Task<IActionResult> GetAsync(
        Guid projectId,
        Guid documentId,
        CancellationToken cancellationToken)
    {
        var result = await getHandler.HandleAsync(
            new GetDocumentQuery(projectId, documentId),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpGet]
    [HasPermission(Permissions.Documents.View)]
    public async Task<IActionResult> ListAsync(
        Guid projectId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 25,
        CancellationToken cancellationToken = default)
    {
        var result = await listHandler.HandleAsync(
            new GetDocumentsQuery(
                projectId,
                new PageRequest(pageNumber, pageSize)),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPut("{documentId:guid}")]
    [HasPermission(Permissions.Documents.Manage)]
    public async Task<IActionResult> UpdateAsync(
        Guid projectId,
        Guid documentId,
        UpdateDocumentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await updateHandler.HandleAsync(
            new UpdateDocumentCommand(
                projectId,
                documentId,
                request.Title,
                request.Category,
                request.Description),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPost("{documentId:guid}/archive")]
    [HasPermission(Permissions.Documents.Manage)]
    public async Task<IActionResult> ArchiveAsync(
        Guid projectId,
        Guid documentId,
        CancellationToken cancellationToken)
    {
        var result = await archiveHandler.HandleAsync(
            new ArchiveDocumentCommand(projectId, documentId),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPost("{documentId:guid}/revisions")]
    [Consumes("multipart/form-data")]
    [HasPermission(Permissions.Documents.Manage)]
    public async Task<IActionResult> AddRevisionAsync(
        Guid projectId,
        Guid documentId,
        [FromForm] UploadDocumentRevisionRequest request,
        CancellationToken cancellationToken)
    {
        await using Stream content = request.File.OpenReadStream();

        var result = await addRevisionHandler.HandleAsync(
            new AddDocumentRevisionCommand(
                projectId,
                documentId,
                request.RevisionCode,
                request.Notes,
                new FileUpload(
                    content,
                    request.File.FileName,
                    NormalizeContentType(request.File.ContentType),
                    request.File.Length)),
            cancellationToken);

        return result.IsSuccess
            ? StatusCode(StatusCodes.Status201Created, result.Value)
            : result.ToActionResult();
    }

    [HttpGet("{documentId:guid}/revisions/{revisionId:guid}/download")]
    [HasPermission(Permissions.Documents.View)]
    public async Task<IActionResult> DownloadRevisionAsync(
        Guid projectId,
        Guid documentId,
        Guid revisionId,
        CancellationToken cancellationToken)
    {
        var result = await downloadRevisionHandler.HandleAsync(
            new DownloadDocumentRevisionQuery(
                projectId,
                documentId,
                revisionId),
            cancellationToken);

        return result.IsFailure
            ? result.ToActionResult()
            : File(
                result.Value.Content,
                result.Value.ContentType,
                result.Value.FileName);
    }

    private static string NormalizeContentType(string contentType) =>
        string.IsNullOrWhiteSpace(contentType)
            ? "application/octet-stream"
            : contentType;
}
