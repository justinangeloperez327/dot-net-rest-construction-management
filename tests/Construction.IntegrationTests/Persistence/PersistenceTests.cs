using Construction.Domain.Projects;
using Construction.Infrastructure.Persistence;
using Construction.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Construction.IntegrationTests.Persistence;

[Collection(IntegrationTestCollection.Name)]
public sealed class PersistenceTests(
    IntegrationTestFixture fixture)
{
    [Fact]
    public async Task ProjectNumber_IsUniqueInPostgreSql()
    {
        string number =
            $"UT-{Guid.NewGuid():N}";

        await using AsyncServiceScope scope =
            fixture.Factory.Services.CreateAsyncScope();

        ApplicationDbContext dbContext =
            scope.ServiceProvider
                .GetRequiredService<ApplicationDbContext>();

        dbContext.Projects.Add(
            CreateProject(number, "First"));

        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        dbContext.Projects.Add(
            CreateProject(number, "Duplicate"));

        await Assert.ThrowsAsync<DbUpdateException>(
            () => dbContext.SaveChangesAsync());
    }

    [Fact]
    public async Task StaleAggregateUpdate_ThrowsConcurrencyException()
    {
        string number =
            $"CC-{Guid.NewGuid():N}";

        Guid projectId;

        await using (AsyncServiceScope createScope =
            fixture.Factory.Services.CreateAsyncScope())
        {
            ApplicationDbContext createContext =
                createScope.ServiceProvider
                    .GetRequiredService<ApplicationDbContext>();

            Project project =
                CreateProject(number, "Original");

            createContext.Projects.Add(project);
            await createContext.SaveChangesAsync();

            projectId = project.Id;

            Assert.Equal(1L, project.Version);
        }

        await using AsyncServiceScope firstScope =
            fixture.Factory.Services.CreateAsyncScope();

        await using AsyncServiceScope secondScope =
            fixture.Factory.Services.CreateAsyncScope();

        ApplicationDbContext firstContext =
            firstScope.ServiceProvider
                .GetRequiredService<ApplicationDbContext>();

        ApplicationDbContext secondContext =
            secondScope.ServiceProvider
                .GetRequiredService<ApplicationDbContext>();

        Project first =
            await firstContext.Projects.SingleAsync(
                project => project.Id == projectId);

        Project stale =
            await secondContext.Projects.SingleAsync(
                project => project.Id == projectId);

        first.Update(
            "First update",
            null,
            first.StartDate,
            first.PlannedEndDate,
            null,
            null,
            null);

        await firstContext.SaveChangesAsync();

        Assert.Equal(2L, first.Version);

        stale.Update(
            "Stale update",
            null,
            stale.StartDate,
            stale.PlannedEndDate,
            null,
            null,
            null);

        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(
            () => secondContext.SaveChangesAsync());
    }

    private static Project CreateProject(
        string number,
        string name) =>
        Project.Create(
            number,
            name,
            null,
            new DateOnly(2026, 1, 1),
            new DateOnly(2026, 12, 31),
            null,
            null,
            null);
}
