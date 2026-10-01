using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;
using Construction.Domain.Rfis;

namespace Construction.Application.Rfis.CreateRfi;

public sealed class CreateRfiCommandHandler(
    IProjectRepository projects,
    IProjectMemberRepository members,
    IRfiRepository rfis,
    IApplicationDbContext dbContext,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService,
    TimeProvider timeProvider)
    : ICommandHandler<CreateRfiCommand, RfiResponse>
{
    public async Task<Result<RfiResponse>> HandleAsync(
        CreateRfiCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

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

        if (await projects.GetAsync(command.ProjectId, cancellationToken) is null)
        {
            return Result.Failure<RfiResponse>(
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
            return Result.Failure<RfiResponse>(responsibleError);
        }

        DateOnly today =
            DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);

        if (command.DueDate is DateOnly dueDate && dueDate < today)
        {
            return Result.Failure<RfiResponse>(
                ApplicationError.Validation(
                    "Rfis.InvalidDueDate",
                    "RFI due date cannot be in the past."));
        }

        string normalizedNumber =
            command.Number.Trim().ToUpperInvariant();

        if (await rfis.ExistsByNumberAsync(
            command.ProjectId,
            normalizedNumber,
            cancellationToken))
        {
            return Result.Failure<RfiResponse>(
                ApplicationError.Conflict(
                    "Rfis.NumberAlreadyExists",
                    "An RFI with the same number already exists in this project."));
        }

        Rfi rfi = Rfi.Create(
            command.ProjectId,
            command.Number,
            command.Subject,
            command.Question,
            command.DueDate,
            command.ResponsibleUserId,
            userId,
            timeProvider.GetUtcNow());

        rfis.Add(rfi);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(RfiResponse.FromDomain(rfi));
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
                "Rfis.InvalidResponsibleUser",
                "The responsible user must be an active project member.")
            : null;
    }
}
