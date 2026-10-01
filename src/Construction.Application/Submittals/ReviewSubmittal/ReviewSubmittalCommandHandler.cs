using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;

namespace Construction.Application.Submittals.ReviewSubmittal;

public sealed class ReviewSubmittalCommandHandler(
    ISubmittalRepository submittals,
    IApplicationDbContext dbContext,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService,
    TimeProvider timeProvider)
    : ICommandHandler<ReviewSubmittalCommand, SubmittalResponse>
{
    public async Task<Result<SubmittalResponse>> HandleAsync(
        ReviewSubmittalCommand command,
        CancellationToken cancellationToken = default)
    {
        ApplicationError? accessError =
            await ProjectAccessGuard.CheckAsync(
                currentUser,
                projectAccessService,
                command.ProjectId,
                Permissions.Submittals.Review,
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

        submittal.ReviewCurrentRevision(
            command.Outcome,
            userId,
            timeProvider.GetUtcNow(),
            command.Remarks);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(SubmittalResponse.FromDomain(submittal));
    }
}
