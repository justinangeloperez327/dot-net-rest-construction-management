using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;

namespace Construction.Application.Equipment.UpdateEquipment;

public sealed class UpdateEquipmentCommandHandler(
    IEquipmentRepository equipment,
    IApplicationDbContext dbContext,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService)
    : ICommandHandler<UpdateEquipmentCommand, EquipmentResponse>
{
    public async Task<Result<EquipmentResponse>> HandleAsync(
        UpdateEquipmentCommand command,
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

        var entity = await equipment.GetAsync(
            command.EquipmentId,
            cancellationToken);

        if (entity is null || entity.ProjectId != command.ProjectId)
        {
            return Result.Failure<EquipmentResponse>(
                ApplicationError.NotFound(
                    "Equipment.NotFound",
                    "The equipment was not found."));
        }

        entity.Update(
            command.Name,
            command.Make,
            command.Model,
            command.SerialNumber);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(EquipmentResponse.FromDomain(entity));
    }
}
