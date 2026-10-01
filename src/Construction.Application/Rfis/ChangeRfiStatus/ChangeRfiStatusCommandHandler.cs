using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;
using Construction.Domain.Rfis;

namespace Construction.Application.Rfis.ChangeRfiStatus;

public sealed class ChangeRfiStatusCommandHandler(
    IRfiRepository rfis,
    IApplicationDbContext dbContext,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService,
    TimeProvider timeProvider)
    : ICommandHandler<ChangeRfiStatusCommand, RfiResponse>
{
    public async Task<Result<RfiResponse>> HandleAsync(
        ChangeRfiStatusCommand command,
        CancellationToken cancellationToken = default)
    {
        ApplicationError? accessError =
            await ProjectAccessGuard.CheckAsync(
                currentUser,
                projectAccessService,
                command.ProjectId,
                Permissions.Rfis.Manage,
                cancellationToken);

        if (accessError is not null)
        {
            return Result.Failure<RfiResponse>(accessError);
        }

        if (currentUser.UserId is not Guid userId)
        {
            return Result.Failure<RfiResponse>(
                ApplicationError.Unauthorized(
                    "Authentication.Required",
                    "Authentication is required."));
        }

        var rfi = await rfis.GetAsync(command.RfiId, cancellationToken);

        if (rfi is null || rfi.ProjectId != command.ProjectId)
        {
            return Result.Failure<RfiResponse>(
                ApplicationError.NotFound(
                    "Rfis.NotFound",
                    "The RFI was not found."));
        }

        DateTimeOffset now = timeProvider.GetUtcNow();

        switch (command.Status)
        {
            case RfiStatus.Open:
                rfi.Reopen(userId, now, command.Reason);
                break;
            case RfiStatus.Closed:
                rfi.Close(userId, now);
                break;
            case RfiStatus.Cancelled:
                rfi.Cancel(userId, now, command.Reason);
                break;
            default:
                return Result.Failure<RfiResponse>(
                    ApplicationError.Validation(
                        "Rfis.InvalidStatusTransition",
                        "The requested RFI status transition is not supported."));
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(RfiResponse.FromDomain(rfi));
    }
}
