using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;

namespace Construction.Application.Submittals.SubmitSubmittal;

public sealed class SubmitSubmittalCommandHandler(
    ISubmittalRepository submittals,
    IApplicationDbContext dbContext,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService,
    TimeProvider timeProvider)
    : ICommandHandler<SubmitSubmittalCommand, SubmittalResponse>
{
    public async Task<Result<SubmittalResponse>> HandleAsync(
        SubmitSubmittalCommand command,
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
            return Result.Failure<SubmittalResponse>(accessError);
        }

        if (currentUser.UserId is not Guid userId)
        {
            return Result.Failure<SubmittalResponse>(
                ApplicationError.Unauthorized(
                    "Authentication.Required",
                    "Authentication is required."));
        }

        var submittal = await submittals.GetAsync(
            command.SubmittalId,
            cancellationToken);

        if (submittal is null || submittal.ProjectId != command.ProjectId)
        {
            return Result.Failure<SubmittalResponse>(
                ApplicationError.NotFound(
                    "Submittals.NotFound",
                    "The submittal was not found."));
        }

        DateOnly today =
            DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);

        if (command.ReviewDueDate is DateOnly reviewDueDate
            && reviewDueDate < today)
        {
            return Result.Failure<SubmittalResponse>(
                ApplicationError.Validation(
                    "Submittals.InvalidReviewDueDate",
                    "Review due date cannot be in the past."));
        }

        submittal.SubmitCurrentRevision(
            userId,
            timeProvider.GetUtcNow(),
            command.ReviewDueDate);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(SubmittalResponse.FromDomain(submittal));
    }
}
