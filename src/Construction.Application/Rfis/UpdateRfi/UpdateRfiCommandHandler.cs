using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;
using Construction.Application.Rfis.CreateRfi;

namespace Construction.Application.Rfis.UpdateRfi;

public sealed class UpdateRfiCommandHandler(
    IRfiRepository rfis,
    IProjectMemberRepository members,
    IApplicationDbContext dbContext,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService,
    TimeProvider timeProvider)
    : ICommandHandler<UpdateRfiCommand, RfiResponse>
{
    public async Task<Result<RfiResponse>> HandleAsync(
        UpdateRfiCommand command,
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

        ApplicationError? responsibleError =
            await CreateRfiCommandHandler.ValidateResponsibleUserAsync(
                command.ProjectId,
                command.ResponsibleUserId,
                members,
                cancellationToken);

        if (responsibleError is not null)
        {
            return Result.Failure<RfiResponse>(responsibleError);
        }

        rfi.Update(
            command.Subject,
            command.Question,
            command.DueDate,
            command.ResponsibleUserId,
            userId,
            timeProvider.GetUtcNow());

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(RfiResponse.FromDomain(rfi));
    }
}
