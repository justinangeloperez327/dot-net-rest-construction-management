using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;

namespace Construction.Application.Equipment.CreateEquipment;

public sealed class CreateEquipmentCommandHandler(
    IProjectRepository projects,
    IEquipmentRepository equipment,
    IApplicationDbContext dbContext,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService)
    : ICommandHandler<CreateEquipmentCommand, EquipmentResponse>
{
    public async Task<Result<EquipmentResponse>> HandleAsync(
        CreateEquipmentCommand command,
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

        if (await projects.GetAsync(command.ProjectId, cancellationToken) is null)
        {
            return Result.Failure<EquipmentResponse>(
                ApplicationError.NotFound(
                    "Projects.NotFound",
                    "The project was not found."));
        }

        string normalizedCode = command.AssetCode.Trim().ToUpperInvariant();

        if (await equipment.ExistsByAssetCodeAsync(
            command.ProjectId,
            normalizedCode,
            cancellationToken))
        {
            return Result.Failure<EquipmentResponse>(
                ApplicationError.Conflict(
                    "Equipment.AssetCodeAlreadyExists",
                    "Equipment asset code already exists in this project."));
        }

        var entity = Construction.Domain.Equipment.Equipment.Create(
            command.ProjectId,
            command.AssetCode,
            command.Name,
            command.Make,
            command.Model,
            command.SerialNumber);

        equipment.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(EquipmentResponse.FromDomain(entity));
    }
}
