using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;
using Construction.Domain.Submittals;

namespace Construction.Application.Submittals.ChangeSubmittalStatus;

public sealed class ChangeSubmittalStatusCommandHandler(
    ISubmittalRepository submittals,
    IApplicationDbContext dbContext,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService,
    TimeProvider timeProvider)
    : ICommandHandler<ChangeSubmittalStatusCommand, SubmittalResponse>
{
    public async Task<Result<SubmittalResponse>> HandleAsync(
        ChangeSubmittalStatusCommand command,
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

        DateTimeOffset now = timeProvider.GetUtcNow();

        switch (command.Status)
        {
            case SubmittalStatus.Closed:
                submittal.Close(userId, now);
                break;
            case SubmittalStatus.Cancelled:
                submittal.Cancel(userId, now, command.Reason);
                break;
            default:
                return Result.Failure<SubmittalResponse>(
                    ApplicationError.Validation(
                        "Submittals.InvalidStatusTransition",
                        "The requested submittal status transition is not supported."));
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(SubmittalResponse.FromDomain(submittal));
    }
}
