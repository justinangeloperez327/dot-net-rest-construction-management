using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;
using Construction.Domain.Submittals;

namespace Construction.Application.Submittals.CreateSubmittal;

public sealed class CreateSubmittalCommandHandler(
    IProjectRepository projects,
    IProjectMemberRepository members,
    ISubmittalRepository submittals,
    IApplicationDbContext dbContext,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService,
    TimeProvider timeProvider)
    : ICommandHandler<CreateSubmittalCommand, SubmittalResponse>
{
    public async Task<Result<SubmittalResponse>> HandleAsync(
        CreateSubmittalCommand command,
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

        if (await projects.GetAsync(command.ProjectId, cancellationToken) is null)
        {
            return Result.Failure<SubmittalResponse>(
                ApplicationError.NotFound(
                    "Projects.NotFound",
                    "The project was not found."));
        }

        ApplicationError? responsibleError =
            await ValidateResponsibleUserAsync(
                command.ProjectId,
                command.ResponsibleUserId,
                members,
                cancellationToken);

        if (responsibleError is not null)
        {
            return Result.Failure<SubmittalResponse>(responsibleError);
        }

        string normalizedNumber =
            command.Number.Trim().ToUpperInvariant();

        if (await submittals.ExistsByNumberAsync(
            command.ProjectId,
            normalizedNumber,
            cancellationToken))
        {
            return Result.Failure<SubmittalResponse>(
                ApplicationError.Conflict(
                    "Submittals.NumberAlreadyExists",
                    "A submittal with the same number already exists in this project."));
        }

        Submittal submittal = Submittal.Create(
            command.ProjectId,
            command.Number,
            command.Title,
            command.Type,
            command.ResponsibleUserId,
            userId,
            timeProvider.GetUtcNow());

        submittals.Add(submittal);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(SubmittalResponse.FromDomain(submittal));
    }

    internal static async Task<ApplicationError?> ValidateResponsibleUserAsync(
        Guid projectId,
        Guid? responsibleUserId,
        IProjectMemberRepository members,
        CancellationToken cancellationToken)
    {
        if (responsibleUserId is not Guid userId)
        {
            return null;
        }

        var member = await members.GetAsync(
            projectId,
            userId,
            cancellationToken);

        return member is null || !member.IsActive
            ? ApplicationError.Validation(
                "Submittals.InvalidResponsibleUser",
                "The responsible user must be an active project member.")
            : null;
    }
}
