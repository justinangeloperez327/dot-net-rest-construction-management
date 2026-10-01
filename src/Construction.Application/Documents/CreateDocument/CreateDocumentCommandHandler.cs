using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;
using Construction.Domain.Documents;

namespace Construction.Application.Documents.CreateDocument;

public sealed class CreateDocumentCommandHandler(
    IProjectRepository projects,
    IDocumentRepository documents,
    IApplicationDbContext dbContext,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService)
    : ICommandHandler<CreateDocumentCommand, DocumentResponse>
{
    public async Task<Result<DocumentResponse>> HandleAsync(
        CreateDocumentCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        ApplicationError? accessError =
            await ProjectAccessGuard.CheckAsync(
                currentUser,
                projectAccessService,
                command.ProjectId,
                Permissions.Documents.Manage,
                cancellationToken);

        if (accessError is not null)
        {
            return Result.Failure<DocumentResponse>(accessError);
        }

        if (currentUser.UserId is not Guid userId)
        {
            return Result.Failure<DocumentResponse>(
                ApplicationError.Unauthorized(
                    "Authentication.Required",
                    "Authentication is required."));
        }

        if (await projects.GetAsync(command.ProjectId, cancellationToken) is null)
        {
            return Result.Failure<DocumentResponse>(
                ApplicationError.NotFound(
                    "Projects.NotFound",
                    "The project was not found."));
        }

        string normalizedNumber =
            command.Number.Trim().ToUpperInvariant();

        if (await documents.ExistsByNumberAsync(
            command.ProjectId,
            normalizedNumber,
            cancellationToken))
        {
            return Result.Failure<DocumentResponse>(
                ApplicationError.Conflict(
                    "Documents.NumberAlreadyExists",
                    "A document with the same number already exists in this project."));
        }

        ProjectDocument document = ProjectDocument.Create(
            command.ProjectId,
            command.Number,
            command.Title,
            command.Category,
            command.Description,
            userId);

        documents.Add(document);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(DocumentResponse.FromDomain(document));
    }
}
