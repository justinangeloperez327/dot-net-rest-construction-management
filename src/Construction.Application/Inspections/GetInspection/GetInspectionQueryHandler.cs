using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;

namespace Construction.Application.Inspections.GetInspection;

public sealed class GetInspectionQueryHandler(
    IInspectionRepository inspections,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService)
    : IQueryHandler<GetInspectionQuery, InspectionResponse>
{
    public async Task<Result<InspectionResponse>> HandleAsync(
        GetInspectionQuery query,
        CancellationToken cancellationToken = default)
    {
        ApplicationError? accessError =
            await ProjectAccessGuard.CheckAsync(
                currentUser,
                projectAccessService,
                query.ProjectId,
                Permissions.Inspections.View,
                cancellationToken);

        if (accessError is not null)
        {
            return Result.Failure<InspectionResponse>(accessError);
        }

        var inspection = await inspections.GetAsync(
            query.InspectionId,
            cancellationToken);

        return inspection is null
            || inspection.ProjectId != query.ProjectId
            ? Result.Failure<InspectionResponse>(
                ApplicationError.NotFound(
                    "Inspections.NotFound",
                    "The inspection was not found."))
            : Result.Success(
                InspectionResponse.FromDomain(inspection));
    }
}
