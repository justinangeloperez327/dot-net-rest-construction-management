using Construction.Api.Authorization;
using Construction.Api.Configuration;
using Construction.Api.Contracts.Documents;
using Construction.Api.Extensions;
using Construction.Application.Abstractions.Files;
using Construction.Application.Attachments.DeleteAttachment;
using Construction.Application.Attachments.DownloadAttachment;
using Construction.Application.Attachments.GetAttachments;
using Construction.Application.Attachments.UploadAttachment;
using Construction.Application.Common.Authorization;
using Construction.Domain.Attachments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Construction.Api.Controllers.V1;

[ApiController]
[Authorize]
[Route(ApiRoutes.Version1 + "/projects/{projectId:guid}/attachments")]
public sealed class AttachmentsController(
    UploadAttachmentCommandHandler uploadHandler,
    GetAttachmentsQueryHandler listHandler,
    DownloadAttachmentQueryHandler downloadHandler,
    DeleteAttachmentCommandHandler deleteHandler)
    : ControllerBase
{
    [HttpPost]
    [Consumes("multipart/form-data")]
    [HasPermission(Permissions.Documents.Manage)]
    public async Task<IActionResult> UploadAsync(
        Guid projectId,
        [FromForm] UploadAttachmentRequest request,
        CancellationToken cancellationToken)
    {
        await using Stream content = request.File.OpenReadStream();

        var result = await uploadHandler.HandleAsync(
            new UploadAttachmentCommand(
                projectId,
                request.TargetType,
                request.TargetId,
                request.Description,
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

    [HttpGet]
    [HasPermission(Permissions.Documents.View)]
    public async Task<IActionResult> ListAsync(
        Guid projectId,
        [FromQuery] AttachmentTargetType targetType,
        [FromQuery] Guid targetId,
        CancellationToken cancellationToken)
    {
        var result = await listHandler.HandleAsync(
            new GetAttachmentsQuery(
                projectId,
                targetType,
                targetId),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpGet("{attachmentId:guid}/download")]
    [HasPermission(Permissions.Documents.View)]
    public async Task<IActionResult> DownloadAsync(
        Guid projectId,
        Guid attachmentId,
        CancellationToken cancellationToken)
    {
        var result = await downloadHandler.HandleAsync(
            new DownloadAttachmentQuery(
                projectId,
                attachmentId),
            cancellationToken);

        return result.IsFailure
            ? result.ToActionResult()
            : File(
                result.Value.Content,
                result.Value.ContentType,
                result.Value.FileName);
    }

    [HttpDelete("{attachmentId:guid}")]
    [HasPermission(Permissions.Documents.Manage)]
    public async Task<IActionResult> DeleteAsync(
        Guid projectId,
        Guid attachmentId,
        CancellationToken cancellationToken)
    {
        var result = await deleteHandler.HandleAsync(
            new DeleteAttachmentCommand(
                projectId,
                attachmentId),
            cancellationToken);

        return result.ToActionResult();
    }

    private static string NormalizeContentType(string contentType) =>
        string.IsNullOrWhiteSpace(contentType)
            ? "application/octet-stream"
            : contentType;
}
