using Construction.Application.Abstractions.Data;
using Construction.Domain.Companies;
using Construction.Domain.Locations;
using Construction.Domain.ProjectMembers;
using Construction.Domain.Projects;
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
