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
            AttachmentTargetType.Project => dbContext.Projects.AsNoTracking()
                .AnyAsync(x => x.Id == targetId && x.Id == projectId, cancellationToken),
            AttachmentTargetType.WorkPackage => dbContext.WorkPackages.AsNoTracking()
                .AnyAsync(x => x.Id == targetId && x.ProjectId == projectId, cancellationToken),
            AttachmentTargetType.Activity => dbContext.Activities.AsNoTracking()
                .AnyAsync(x => x.Id == targetId && x.ProjectId == projectId, cancellationToken),
            AttachmentTargetType.DailyProgressReport => dbContext.DailyProgressReports.AsNoTracking()
                .AnyAsync(x => x.Id == targetId && x.ProjectId == projectId, cancellationToken),
            AttachmentTargetType.Rfi => dbContext.Rfis.AsNoTracking()
                .AnyAsync(x => x.Id == targetId && x.ProjectId == projectId, cancellationToken),
            AttachmentTargetType.Submittal => dbContext.Submittals.AsNoTracking()
                .AnyAsync(x => x.Id == targetId && x.ProjectId == projectId, cancellationToken),
            AttachmentTargetType.SubmittalRevision => dbContext.SubmittalRevisions.AsNoTracking()
                .AnyAsync(x => x.Id == targetId && dbContext.Submittals.Any(s => s.Id == x.SubmittalId && s.ProjectId == projectId), cancellationToken),
            AttachmentTargetType.Inspection => dbContext.Inspections.AsNoTracking()
                .AnyAsync(x => x.Id == targetId && x.ProjectId == projectId, cancellationToken),
            AttachmentTargetType.Issue => dbContext.Issues.AsNoTracking()
                .AnyAsync(x => x.Id == targetId && x.ProjectId == projectId, cancellationToken),
            AttachmentTargetType.CorrectiveAction => dbContext.CorrectiveActions.AsNoTracking()
                .AnyAsync(x => x.Id == targetId && dbContext.Issues.Any(i => i.Id == x.IssueId && i.ProjectId == projectId), cancellationToken),
            AttachmentTargetType.Equipment => dbContext.Equipment.AsNoTracking()
                .AnyAsync(x => x.Id == targetId && x.ProjectId == projectId, cancellationToken),
            AttachmentTargetType.EquipmentMaintenance => dbContext.EquipmentMaintenanceRecords.AsNoTracking()
                .AnyAsync(x => x.Id == targetId && dbContext.Equipment.Any(e => e.Id == x.EquipmentId && e.ProjectId == projectId), cancellationToken),
            AttachmentTargetType.PurchaseRequest => dbContext.PurchaseRequests.AsNoTracking()
                .AnyAsync(x => x.Id == targetId && x.ProjectId == projectId, cancellationToken),
            AttachmentTargetType.PurchaseOrder => dbContext.PurchaseOrders.AsNoTracking()
                .AnyAsync(x => x.Id == targetId && x.ProjectId == projectId, cancellationToken),
            _ => Task.FromResult(false)
        };
}
