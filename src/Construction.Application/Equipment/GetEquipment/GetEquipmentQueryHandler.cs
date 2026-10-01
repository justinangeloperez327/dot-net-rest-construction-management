using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;

namespace Construction.Application.Equipment.GetEquipment;

public sealed class GetEquipmentQueryHandler(
    IEquipmentRepository equipment,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService)
    : IQueryHandler<GetEquipmentQuery, EquipmentResponse>
{
    public async Task<Result<EquipmentResponse>> HandleAsync(
        GetEquipmentQuery query,
        CancellationToken cancellationToken = default)
    {
        ApplicationError? accessError = await ProjectAccessGuard.CheckAsync(
            currentUser,
            projectAccessService,
            query.ProjectId,
            Permissions.Equipment.View,
            cancellationToken);

        if (accessError is not null)
        {
            return Result.Failure<EquipmentResponse>(accessError);
        }

        var entity = await equipment.GetAsync(
            query.EquipmentId,
            cancellationToken);

        return entity is null || entity.ProjectId != query.ProjectId
            ? Result.Failure<EquipmentResponse>(
                ApplicationError.NotFound(
                    "Equipment.NotFound",
                    "The equipment was not found."))
            : Result.Success(EquipmentResponse.FromDomain(entity));
    }
}
