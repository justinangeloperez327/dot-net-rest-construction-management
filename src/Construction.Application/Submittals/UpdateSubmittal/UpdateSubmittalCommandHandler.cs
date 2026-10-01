using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;
using Construction.Application.Submittals.CreateSubmittal;

namespace Construction.Application.Submittals.UpdateSubmittal;

public sealed class UpdateSubmittalCommandHandler(
    ISubmittalRepository submittals,
    IProjectMemberRepository members,
    IApplicationDbContext dbContext,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService,
    TimeProvider timeProvider)
    : ICommandHandler<UpdateSubmittalCommand, SubmittalResponse>
{
    public async Task<Result<SubmittalResponse>> HandleAsync(
        UpdateSubmittalCommand command,
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

        ApplicationError? responsibleError =
            await CreateSubmittalCommandHandler.ValidateResponsibleUserAsync(
                command.ProjectId,
                command.ResponsibleUserId,
                members,
                cancellationToken);

        if (responsibleError is not null)
        {
            return Result.Failure<SubmittalResponse>(responsibleError);
        }

        submittal.Update(
            command.Title,
            command.Type,
            command.ResponsibleUserId,
            userId,
            timeProvider.GetUtcNow());

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(SubmittalResponse.FromDomain(submittal));
    }
}
