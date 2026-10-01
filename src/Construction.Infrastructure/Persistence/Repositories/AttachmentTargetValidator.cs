using Construction.Application.Abstractions.Data;
using Construction.Domain.Attachments;
using Microsoft.EntityFrameworkCore;

namespace Construction.Infrastructure.Persistence.Repositories;

public sealed class AttachmentTargetValidator(ApplicationDbContext dbContext)
    : IAttachmentTargetValidator
{
    public Task<bool> ExistsAsync(
        Guid projectId,
        AttachmentTargetType targetType,
        Guid targetId,
        CancellationToken cancellationToken = default) =>
        targetType switch
        {
            AttachmentTargetType.Project =>
                dbContext.Projects
                    .AsNoTracking()
                    .AnyAsync(
                        project =>
                            project.Id == targetId
                            && project.Id == projectId,
                        cancellationToken),

            AttachmentTargetType.WorkPackage =>
                dbContext.WorkPackages
                    .AsNoTracking()
                    .AnyAsync(
                        workPackage =>
                            workPackage.Id == targetId
                            && workPackage.ProjectId == projectId,
                        cancellationToken),

            AttachmentTargetType.Activity =>
                dbContext.Activities
                    .AsNoTracking()
                    .AnyAsync(
                        activity =>
                            activity.Id == targetId
                            && activity.ProjectId == projectId,
                        cancellationToken),

            AttachmentTargetType.DailyProgressReport =>
                dbContext.DailyProgressReports
                    .AsNoTracking()
                    .AnyAsync(
                        report =>
                            report.Id == targetId
                            && report.ProjectId == projectId,
                        cancellationToken),

            _ => Task.FromResult(false)
        };
}
