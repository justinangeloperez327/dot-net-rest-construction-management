using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;
using Construction.Domain.Equipment;

namespace Construction.Application.Equipment.ChangeEquipmentStatus;

public sealed class ChangeEquipmentStatusCommandHandler(
    IEquipmentRepository equipment,
    IApplicationDbContext dbContext,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService)
    : ICommandHandler<ChangeEquipmentStatusCommand, EquipmentResponse>
{
    public async Task<Result<EquipmentResponse>> HandleAsync(
        ChangeEquipmentStatusCommand command,
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
            return Result.Failure<EquipmentResponse>(accessError);
        }

        var entity = await equipment.GetAsync(command.EquipmentId, cancellationToken);

        if (entity is null || entity.ProjectId != command.ProjectId)
        {
            return Result.Failure<EquipmentResponse>(
                ApplicationError.NotFound("Equipment.NotFound", "The equipment was not found."));
        }

        switch (command.Status)
        {
            case EquipmentStatus.OutOfService:
                entity.MarkOutOfService();
                break;
            case EquipmentStatus.Available:
                entity.RestoreToAvailable();
                break;
            case EquipmentStatus.Retired:
                entity.Retire();
                break;
            default:
                return Result.Failure<EquipmentResponse>(
                    ApplicationError.Validation(
                        "Equipment.InvalidStatusTransition",
                        "The requested equipment status transition is not supported."));
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(EquipmentResponse.FromDomain(entity));
    }
}
