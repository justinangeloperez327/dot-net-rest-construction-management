using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;

namespace Construction.Application.Rfis.OpenRfi;

public sealed class OpenRfiCommandHandler(
    IRfiRepository rfis,
    IApplicationDbContext dbContext,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService,
    TimeProvider timeProvider)
    : ICommandHandler<OpenRfiCommand, RfiResponse>
{
    public async Task<Result<RfiResponse>> HandleAsync(
        OpenRfiCommand command,
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

        rfi.Open(userId, timeProvider.GetUtcNow());
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(RfiResponse.FromDomain(rfi));
    }
}
