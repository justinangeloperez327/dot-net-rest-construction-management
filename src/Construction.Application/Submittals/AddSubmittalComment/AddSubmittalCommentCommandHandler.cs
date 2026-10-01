using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;

namespace Construction.Application.Submittals.AddSubmittalComment;

public sealed class AddSubmittalCommentCommandHandler(
    ISubmittalRepository submittals,
    IApplicationDbContext dbContext,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService,
    TimeProvider timeProvider)
    : ICommandHandler<AddSubmittalCommentCommand, SubmittalCommentResponse>
{
    public async Task<Result<SubmittalCommentResponse>> HandleAsync(
        AddSubmittalCommentCommand command,
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
            return Result.Failure<SubmittalCommentResponse>(accessError);
        }

        if (currentUser.UserId is not Guid userId)
        {
            return Result.Failure<SubmittalCommentResponse>(
                ApplicationError.Unauthorized(
                    "Authentication.Required",
                    "Authentication is required."));
        }

        var submittal = await submittals.GetAsync(
            command.SubmittalId,
            cancellationToken);

        if (submittal is null || submittal.ProjectId != command.ProjectId)
        {
            return Result.Failure<SubmittalCommentResponse>(
                ApplicationError.NotFound(
                    "Submittals.NotFound",
                    "The submittal was not found."));
        }

        var comment = submittal.AddComment(
            userId,
            command.Body,
            timeProvider.GetUtcNow());

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(
            SubmittalCommentResponse.FromDomain(comment));
    }
}
