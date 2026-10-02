using System.Globalization;
using Construction.Infrastructure.Identity;
using Construction.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using Xunit;

namespace Construction.IntegrationTests.Infrastructure;

public sealed class IntegrationTestFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres =
        new PostgreSqlBuilder("postgres:18-alpine")
            .WithDatabase("construction_tests")
            .WithUsername("construction")
            .WithPassword("construction")
            .Build();

    private string _fileStorageRoot = string.Empty;

    public ConstructionWebApplicationFactory Factory { get; private set; } =
        null!;

    public string UserEmail { get; } =
        "integration.user@example.test";

    public string UserPassword { get; } =
        "Integration-Test-Password1!";

    public async ValueTask InitializeAsync()
    {
        await _postgres.StartAsync();

        _fileStorageRoot = Path.Combine(
            Path.GetTempPath(),
            "construction-integration-tests",
            Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(_fileStorageRoot);

        Factory = new ConstructionWebApplicationFactory(
            _postgres.GetConnectionString(),
            _fileStorageRoot);

        await using AsyncServiceScope scope =
            Factory.Services.CreateAsyncScope();

        ApplicationDbContext dbContext =
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (dbContext.Database.HasPendingModelChanges())
        {
            IMigrationsAssembly migrationsAssembly =
                dbContext.GetService<IMigrationsAssembly>();

            IDesignTimeModel designTimeModel =
                dbContext.GetService<IDesignTimeModel>();

            IMigrationsModelDiffer modelDiffer =
                dbContext.GetService<IMigrationsModelDiffer>();

            ModelSnapshot? snapshot =
                migrationsAssembly.ModelSnapshot;

            string detail = "Unknown model difference.";

            if (snapshot is not null)
            {
                IModelRuntimeInitializer runtimeInitializer =
                    dbContext.GetService<IModelRuntimeInitializer>();

                IModel initializedSnapshot =
                    runtimeInitializer.Initialize(
                        snapshot.Model,
                        designTime: true,
                        validationLogger: null);

                var differences = modelDiffer.GetDifferences(
                    initializedSnapshot.GetRelationalModel(),
                    designTimeModel.Model.GetRelationalModel());

                detail = string.Join(
                    " | ",
                    differences.Select(DescribeDifference));
            }

            throw new InvalidOperationException(
                $"Runtime EF model differs from the migration snapshot: {detail}");
        }

        DatabaseInitializer initializer =
            scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();

        await initializer.InitializeAsync(
            TestContext.Current.CancellationToken);

        UserManager<ApplicationUser> userManager =
            scope.ServiceProvider
                .GetRequiredService<UserManager<ApplicationUser>>();

        ApplicationUser? existing =
            await userManager.FindByEmailAsync(UserEmail);

        if (existing is null)
        {
            var user = new ApplicationUser
            {
                Id = Guid.CreateVersion7(),
                UserName = UserEmail,
                Email = UserEmail,
                EmailConfirmed = true,
                DisplayName = "Integration User",
                IsActive = true
            };

            IdentityResult result =
                await userManager.CreateAsync(
                    user,
                    UserPassword);

            if (!result.Succeeded)
            {
                string errors = string.Join(
                    "; ",
                    result.Errors.Select(error =>
                        $"{error.Code}: {error.Description}"));

                throw new InvalidOperationException(
                    $"Unable to create integration test user: {errors}");
            }
        }
    }

    private static string DescribeDifference(
        MigrationOperation operation) =>
        operation switch
        {
            AlterColumnOperation column =>
                $"AlterColumn {column.Table}.{column.Name}: "
                + $"oldType={column.OldColumn.ColumnType ?? "<null>"}, "
                + $"newType={column.ColumnType ?? "<null>"}, "
                + $"oldNullable={column.OldColumn.IsNullable}, "
                + $"newNullable={column.IsNullable}, "
                + $"oldMaxLength={column.OldColumn.MaxLength?.ToString(CultureInfo.InvariantCulture) ?? "<null>"}, "
                + $"newMaxLength={column.MaxLength?.ToString(CultureInfo.InvariantCulture) ?? "<null>"}",
            _ => operation.GetType().Name
        };

    public HttpClient CreateClient() =>
        Factory.CreateClient(
            new WebApplicationFactoryClientOptions
            {
                BaseAddress =
                    new Uri("https://localhost"),
                AllowAutoRedirect = false
            });

    public async ValueTask DisposeAsync()
    {
        Factory.Dispose();
        await _postgres.DisposeAsync();

        if (Directory.Exists(_fileStorageRoot))
        {
            Directory.Delete(
                _fileStorageRoot,
                recursive: true);
        }
    }
}
