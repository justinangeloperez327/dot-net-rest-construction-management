using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;

namespace Construction.Application.Attachments.GetAttachments;

public sealed class GetAttachmentsQueryHandler(
    IAttachmentRepository attachments,
    IAttachmentTargetValidator targetValidator,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService)
    : IQueryHandler<GetAttachmentsQuery, IReadOnlyCollection<AttachmentResponse>>
{
    public async Task<Result<IReadOnlyCollection<AttachmentResponse>>> HandleAsync(
        GetAttachmentsQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        ApplicationError? accessError =
            await ProjectAccessGuard.CheckAsync(
                currentUser,
                projectAccessService,
                query.ProjectId,
                Permissions.Documents.View,
                cancellationToken);

        if (accessError is not null)
        {
            return Result.Failure<IReadOnlyCollection<AttachmentResponse>>(
                accessError);
        }

        if (!await targetValidator.ExistsAsync(
            query.ProjectId,
            query.TargetType,
            query.TargetId,
            cancellationToken))
        {
            return Result.Failure<IReadOnlyCollection<AttachmentResponse>>(
                ApplicationError.NotFound(
                    "Attachments.TargetNotFound",
                    "The attachment target was not found."));
        }

        var items = await attachments.GetByTargetAsync(
            query.ProjectId,
            query.TargetType,
            query.TargetId,
            cancellationToken);

        IReadOnlyCollection<AttachmentResponse> response =
            items.Select(AttachmentResponse.FromDomain).ToArray();

        return Result.Success(response);
    }
}
