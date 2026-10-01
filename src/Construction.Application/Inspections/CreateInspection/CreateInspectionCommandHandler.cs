using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Quality;
using Construction.Application.Common.Results;
using Construction.Domain.Inspections;

namespace Construction.Application.Inspections.CreateInspection;

public sealed class CreateInspectionCommandHandler(
    IProjectRepository projects,
    IProjectLocationRepository locations,
    IActivityRepository activities,
    IProjectMemberRepository members,
    IInspectionRepository inspections,
    IApplicationDbContext dbContext,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService,
    TimeProvider timeProvider)
    : ICommandHandler<CreateInspectionCommand, InspectionResponse>
{
    public async Task<Result<InspectionResponse>> HandleAsync(
        CreateInspectionCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        ApplicationError? accessError =
            await ProjectAccessGuard.CheckAsync(
                currentUser,
                projectAccessService,
                command.ProjectId,
                Permissions.Inspections.Manage,
                cancellationToken);

        if (accessError is not null)
        {
            return Result.Failure<InspectionResponse>(accessError);
        }

        if (currentUser.UserId is not Guid userId)
        {
            return Result.Failure<InspectionResponse>(
                ApplicationError.Unauthorized(
                    "Authentication.Required",
                    "Authentication is required."));
        }

        if (await projects.GetAsync(command.ProjectId, cancellationToken) is null)
        {
            return Result.Failure<InspectionResponse>(
                ApplicationError.NotFound(
                    "Projects.NotFound",
                    "The project was not found."));
        }

        ApplicationError? referenceError =
            await QualityReferenceValidator.ValidateLocationAndActivityAsync(
                command.ProjectId,
                command.LocationId,
                command.ActivityId,
                locations,
                activities,
                "Inspections",
                cancellationToken);

        if (referenceError is not null)
        {
            return Result.Failure<InspectionResponse>(referenceError);
        }

        ApplicationError? inspectorError =
            await QualityReferenceValidator.ValidateProjectMemberAsync(
                command.ProjectId,
                command.InspectorUserId,
                members,
                "Inspections.InvalidInspector",
                "The inspector must be an active project member.",
                cancellationToken);

        if (inspectorError is not null)
        {
            return Result.Failure<InspectionResponse>(inspectorError);
        }

        DateOnly today =
            DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);

        if (command.RequestedForDate is DateOnly requestedForDate
            && requestedForDate < today)
        {
            return Result.Failure<InspectionResponse>(
                ApplicationError.Validation(
                    "Inspections.InvalidRequestedDate",
                    "Requested inspection date cannot be in the past."));
        }

        string normalizedNumber =
            command.Number.Trim().ToUpperInvariant();

        if (await inspections.ExistsByNumberAsync(
            command.ProjectId,
            normalizedNumber,
            cancellationToken))
        {
            return Result.Failure<InspectionResponse>(
                ApplicationError.Conflict(
                    "Inspections.NumberAlreadyExists",
                    "An inspection with the same number already exists in this project."));
        }

        Inspection inspection = Inspection.Create(
            command.ProjectId,
            command.Number,
            command.Title,
            command.Description,
            command.Type,
            command.LocationId,
            command.ActivityId,
            command.RequestedForDate,
            command.InspectorUserId,
            userId,
            timeProvider.GetUtcNow());

        inspections.Add(inspection);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(
            InspectionResponse.FromDomain(inspection));
    }
}
