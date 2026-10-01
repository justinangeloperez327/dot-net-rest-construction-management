using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Quality;
using Construction.Application.Common.Results;

namespace Construction.Application.Equipment.AssignEquipment;

public sealed class AssignEquipmentCommandHandler(
    IEquipmentRepository equipment,
    IProjectMemberRepository members,
    IProjectLocationRepository locations,
    IApplicationDbContext dbContext,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService,
    TimeProvider timeProvider)
    : ICommandHandler<AssignEquipmentCommand, EquipmentAssignmentResponse>
{
    public async Task<Result<EquipmentAssignmentResponse>> HandleAsync(
        AssignEquipmentCommand command,
        CancellationToken cancellationToken = default)
    {
        ApplicationError? accessError = await ProjectAccessGuard.CheckAsync(
            currentUser,
            projectAccessService,
            command.ProjectId,
            Permissions.Equipment.Manage,
            cancellationToken);

        if (accessError is not null)
        {
            return Result.Failure<EquipmentAssignmentResponse>(accessError);
        }

        var entity = await equipment.GetAsync(
            command.EquipmentId,
            cancellationToken);

        if (entity is null || entity.ProjectId != command.ProjectId)
        {
            return Result.Failure<EquipmentAssignmentResponse>(
                ApplicationError.NotFound(
                    "Equipment.NotFound",
                    "The equipment was not found."));
        }

        ApplicationError? memberError =
            await QualityReferenceValidator.ValidateProjectMemberAsync(
                command.ProjectId,
                command.UserId,
                members,
                "Equipment.InvalidAssignedUser",
                "The assigned user must be an active project member.",
                cancellationToken);

        if (memberError is not null)
        {
            return Result.Failure<EquipmentAssignmentResponse>(memberError);
        }

        if (command.LocationId is Guid locationId)
        {
            var location = await locations.GetByIdAsync(
                locationId,
                cancellationToken);

            if (location is null || location.ProjectId != command.ProjectId)
            {
                return Result.Failure<EquipmentAssignmentResponse>(
                    ApplicationError.Validation(
                        "Equipment.InvalidLocation",
                        "The equipment location must belong to the same project."));
            }
        }

        var assignment = entity.Assign(
            command.UserId,
            command.LocationId,
            timeProvider.GetUtcNow(),
            command.Notes);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(
            EquipmentAssignmentResponse.FromDomain(assignment));
    }
}
