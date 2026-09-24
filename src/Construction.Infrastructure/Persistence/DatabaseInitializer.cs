using Construction.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;

namespace Construction.Infrastructure.Persistence;

public sealed class DatabaseInitializer(
    ApplicationDbContext dbContext,
    IdentitySeeder identitySeeder)
{
    public async Task InitializeAsync(
        CancellationToken cancellationToken = default)
    {
        IEnumerable<string> pendingMigrations =
            await dbContext.Database.GetPendingMigrationsAsync(
                cancellationToken);

        if (pendingMigrations.Any())
        {
            await dbContext.Database.MigrateAsync(
                cancellationToken);
        }

        await identitySeeder.SeedAsync(cancellationToken);
    }
}
