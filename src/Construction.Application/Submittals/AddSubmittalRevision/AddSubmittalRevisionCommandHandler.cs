using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;

namespace Construction.Application.Submittals.AddSubmittalRevision;

public sealed class AddSubmittalRevisionCommandHandler(
    ISubmittalRepository submittals,
    IApplicationDbContext dbContext,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService,
    TimeProvider timeProvider)
    : ICommandHandler<AddSubmittalRevisionCommand, SubmittalRevisionResponse>
{
    public async Task<Result<SubmittalRevisionResponse>> HandleAsync(
        AddSubmittalRevisionCommand command,
        CancellationToken cancellationToken = default)
    {
        ApplicationError? accessError =
            await ProjectAccessGuard.CheckAsync(
                currentUser,
                projectAccessService,
                command.ProjectId,
                Permissions.Submittals.Manage,
                cancellationToken);

        if (accessError is not null)
        {
            return Result.Failure<SubmittalRevisionResponse>(accessError);
        }

        if (currentUser.UserId is not Guid userId)
        {
            return Result.Failure<SubmittalRevisionResponse>(
                ApplicationError.Unauthorized(
                    "Authentication.Required",
                    "Authentication is required."));
        }

        var submittal = await submittals.GetAsync(
            command.SubmittalId,
            cancellationToken);

        if (submittal is null || submittal.ProjectId != command.ProjectId)
        {
            return Result.Failure<SubmittalRevisionResponse>(
                ApplicationError.NotFound(
                    "Submittals.NotFound",
                    "The submittal was not found."));
        }

        var revision = submittal.AddRevision(
            command.RevisionCode,
            command.Description,
            userId,
            timeProvider.GetUtcNow());

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(
            SubmittalRevisionResponse.FromDomain(revision));
    }
}
