using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;

namespace Construction.Application.Rfis.RespondRfi;

public sealed class RespondRfiCommandHandler(
    IRfiRepository rfis,
    IApplicationDbContext dbContext,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService,
    TimeProvider timeProvider)
    : ICommandHandler<RespondRfiCommand, RfiResponse>
{
    public async Task<Result<RfiResponse>> HandleAsync(
        RespondRfiCommand command,
        CancellationToken cancellationToken = default)
    {
        ApplicationError? accessError =
            await ProjectAccessGuard.CheckAsync(
                currentUser,
                projectAccessService,
                command.ProjectId,
                Permissions.Rfis.Respond,
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

        rfi.Respond(
            command.Response,
            userId,
            timeProvider.GetUtcNow());

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(RfiResponse.FromDomain(rfi));
    }
}
