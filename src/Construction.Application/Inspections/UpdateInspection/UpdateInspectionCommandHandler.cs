using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Quality;
using Construction.Application.Common.Results;

namespace Construction.Application.Inspections.UpdateInspection;

public sealed class UpdateInspectionCommandHandler(
    IInspectionRepository inspections,
    IProjectLocationRepository locations,
    IActivityRepository activities,
    IProjectMemberRepository members,
    IApplicationDbContext dbContext,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService,
    TimeProvider timeProvider)
    : ICommandHandler<UpdateInspectionCommand, InspectionResponse>
{
    public async Task<Result<InspectionResponse>> HandleAsync(
        UpdateInspectionCommand command,
        CancellationToken cancellationToken = default)
    {
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

        var inspection = await inspections.GetAsync(
            command.InspectionId,
            cancellationToken);

        if (inspection is null || inspection.ProjectId != command.ProjectId)
        {
            return Result.Failure<InspectionResponse>(
                ApplicationError.NotFound(
                    "Inspections.NotFound",
                    "The inspection was not found."));
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

        inspection.Update(
            command.Title,
            command.Description,
            command.Type,
            command.LocationId,
            command.ActivityId,
            command.RequestedForDate,
            command.InspectorUserId,
            userId,
            timeProvider.GetUtcNow());

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(
            InspectionResponse.FromDomain(inspection));
    }
}
