using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;

namespace Construction.Application.Equipment.ReturnEquipment;

public sealed class ReturnEquipmentCommandHandler(
    IEquipmentRepository equipment,
    IApplicationDbContext dbContext,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService,
    TimeProvider timeProvider)
    : ICommandHandler<ReturnEquipmentCommand, EquipmentResponse>
{
    public async Task<Result<EquipmentResponse>> HandleAsync(
        ReturnEquipmentCommand command,
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

        entity.Return(timeProvider.GetUtcNow());
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(EquipmentResponse.FromDomain(entity));
    }
}
