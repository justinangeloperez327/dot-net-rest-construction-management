using Construction.Application.Abstractions.Data;
using Construction.Domain.Activities;
using Construction.Domain.Attachments;
using Construction.Domain.Companies;
using Construction.Domain.DailyProgress;
using Construction.Domain.Documents;
using Construction.Domain.Locations;
using Construction.Domain.ProjectMembers;
using Construction.Domain.Projects;
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
