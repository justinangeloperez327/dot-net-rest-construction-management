using Construction.Application.Abstractions.Data;
using Construction.Domain.Activities;
using Construction.Domain.Attachments;
using Construction.Domain.Audit;
using Construction.Domain.Companies;
using Construction.Domain.DailyProgress;
using Construction.Domain.Documents;
using Construction.Domain.Equipment;
using Construction.Domain.Inspections;
using Construction.Domain.Issues;
using Construction.Domain.Locations;
using Construction.Domain.Notifications;
using Construction.Domain.ProjectMembers;
using Construction.Domain.Projects;
using Construction.Domain.PurchaseOrders;
using Construction.Domain.PurchaseRequests;
using Construction.Domain.Rfis;
using Construction.Domain.Submittals;
using Construction.Domain.Suppliers;
using Construction.Domain.WorkPackages;
using Construction.Infrastructure.Authentication;
using Construction.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Construction.Infrastructure.Persistence;

public sealed class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>(options),
      IApplicationDbContext
{
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectMember> ProjectMembers => Set<ProjectMember>();
    public DbSet<ProjectLocation> ProjectLocations => Set<ProjectLocation>();
    public DbSet<WorkPackage> WorkPackages => Set<WorkPackage>();
    public DbSet<Activity> Activities => Set<Activity>();
    public DbSet<ActivityAssignment> ActivityAssignments => Set<ActivityAssignment>();
    public DbSet<ActivityDependency> ActivityDependencies => Set<ActivityDependency>();
    public DbSet<DailyProgressReport> DailyProgressReports => Set<DailyProgressReport>();
    public DbSet<DailyProgressActivity> DailyProgressActivities => Set<DailyProgressActivity>();
    public DbSet<DailyProgressManpower> DailyProgressManpower => Set<DailyProgressManpower>();
    public DbSet<DailyProgressEquipment> DailyProgressEquipment => Set<DailyProgressEquipment>();
    public DbSet<ProjectDocument> Documents => Set<ProjectDocument>();
    public DbSet<DocumentRevision> DocumentRevisions => Set<DocumentRevision>();
    public DbSet<Attachment> Attachments => Set<Attachment>();
    public DbSet<Rfi> Rfis => Set<Rfi>();
    public DbSet<RfiComment> RfiComments => Set<RfiComment>();
    public DbSet<RfiHistoryEntry> RfiHistoryEntries => Set<RfiHistoryEntry>();
    public DbSet<Submittal> Submittals => Set<Submittal>();
    public DbSet<SubmittalRevision> SubmittalRevisions => Set<SubmittalRevision>();
    public DbSet<SubmittalComment> SubmittalComments => Set<SubmittalComment>();
    public DbSet<SubmittalHistoryEntry> SubmittalHistoryEntries => Set<SubmittalHistoryEntry>();
    public DbSet<Inspection> Inspections => Set<Inspection>();
    public DbSet<InspectionHistoryEntry> InspectionHistoryEntries => Set<InspectionHistoryEntry>();
    public DbSet<Issue> Issues => Set<Issue>();
    public DbSet<CorrectiveAction> CorrectiveActions => Set<CorrectiveAction>();
    public DbSet<IssueHistoryEntry> IssueHistoryEntries => Set<IssueHistoryEntry>();
    public DbSet<Equipment> Equipment => Set<Equipment>();
    public DbSet<EquipmentAssignment> EquipmentAssignments => Set<EquipmentAssignment>();
    public DbSet<EquipmentMaintenanceRecord> EquipmentMaintenanceRecords => Set<EquipmentMaintenanceRecord>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<PurchaseRequest> PurchaseRequests => Set<PurchaseRequest>();
    public DbSet<PurchaseRequestItem> PurchaseRequestItems => Set<PurchaseRequestItem>();
    public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
    public DbSet<PurchaseOrderItem> PurchaseOrderItems => Set<PurchaseOrderItem>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<NotificationPreference> NotificationPreferences => Set<NotificationPreference>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly);

        IdentityTableConfiguration.Apply(builder);
    }
}
