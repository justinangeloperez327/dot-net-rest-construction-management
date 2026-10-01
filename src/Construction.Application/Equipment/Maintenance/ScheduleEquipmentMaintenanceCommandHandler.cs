using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;

namespace Construction.Application.Equipment.Maintenance;

public sealed class ScheduleEquipmentMaintenanceCommandHandler(
    IEquipmentRepository equipment,
    IApplicationDbContext dbContext,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService)
    : ICommandHandler<ScheduleEquipmentMaintenanceCommand, EquipmentMaintenanceResponse>
{
    public async Task<Result<EquipmentMaintenanceResponse>> HandleAsync(
        ScheduleEquipmentMaintenanceCommand command,
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
            return Result.Failure<EquipmentMaintenanceResponse>(accessError);
        }

        var entity = await equipment.GetAsync(command.EquipmentId, cancellationToken);

        if (entity is null || entity.ProjectId != command.ProjectId)
        {
            return Result.Failure<EquipmentMaintenanceResponse>(
                ApplicationError.NotFound("Equipment.NotFound", "The equipment was not found."));
        }

        var record = entity.ScheduleMaintenance(
            command.Description,
            command.ScheduledDate,
            command.ServiceProvider);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(EquipmentMaintenanceResponse.FromDomain(record));
    }
}
