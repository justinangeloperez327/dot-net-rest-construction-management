using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;

namespace Construction.Application.Inspections.RequestInspection;

public sealed class RequestInspectionCommandHandler(
    IInspectionRepository inspections,
    IApplicationDbContext dbContext,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService,
    TimeProvider timeProvider)
    : ICommandHandler<RequestInspectionCommand, InspectionResponse>
{
    public async Task<Result<InspectionResponse>> HandleAsync(
        RequestInspectionCommand command,
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

        inspection.Request(userId, timeProvider.GetUtcNow());
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(
            InspectionResponse.FromDomain(inspection));
    }
}
