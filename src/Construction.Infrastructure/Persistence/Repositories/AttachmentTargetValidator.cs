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

            AttachmentTargetType.Rfi =>
                dbContext.Rfis
                    .AsNoTracking()
                    .AnyAsync(
                        rfi =>
                            rfi.Id == targetId
                            && rfi.ProjectId == projectId,
                        cancellationToken),

            AttachmentTargetType.Submittal =>
                dbContext.Submittals
                    .AsNoTracking()
                    .AnyAsync(
                        submittal =>
                            submittal.Id == targetId
                            && submittal.ProjectId == projectId,
                        cancellationToken),

            AttachmentTargetType.SubmittalRevision =>
                dbContext.SubmittalRevisions
                    .AsNoTracking()
                    .AnyAsync(
                        revision =>
                            revision.Id == targetId
                            && dbContext.Submittals.Any(submittal =>
                                submittal.Id == revision.SubmittalId
                                && submittal.ProjectId == projectId),
                        cancellationToken),

            AttachmentTargetType.Inspection =>
                dbContext.Inspections
                    .AsNoTracking()
                    .AnyAsync(
                        inspection =>
                            inspection.Id == targetId
                            && inspection.ProjectId == projectId,
                        cancellationToken),

            AttachmentTargetType.Issue =>
                dbContext.Issues
                    .AsNoTracking()
                    .AnyAsync(
                        issue =>
                            issue.Id == targetId
                            && issue.ProjectId == projectId,
                        cancellationToken),

            AttachmentTargetType.CorrectiveAction =>
                dbContext.CorrectiveActions
                    .AsNoTracking()
                    .AnyAsync(
                        action =>
                            action.Id == targetId
                            && dbContext.Issues.Any(issue =>
                                issue.Id == action.IssueId
                                && issue.ProjectId == projectId),
                        cancellationToken),

            _ => Task.FromResult(false)
        };
}
