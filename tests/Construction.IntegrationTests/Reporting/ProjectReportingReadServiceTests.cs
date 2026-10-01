using Construction.Application.Abstractions.Reports;
using Construction.Domain.Activities;
using Construction.Domain.Projects;
using Construction.Infrastructure.Persistence;
using Construction.IntegrationTests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Construction.IntegrationTests.Reporting;

[Collection(IntegrationTestGroup.Name)]
public sealed class ProjectReportingReadServiceTests(
    IntegrationTestFixture fixture)
{
    [Fact]
    public async Task ActivityReport_AggregatesProgressAndOverdueWork()
    {
        Guid projectId;

        await using (AsyncServiceScope scope =
            fixture.Factory.Services.CreateAsyncScope())
        {
            ApplicationDbContext dbContext =
                scope.ServiceProvider
                    .GetRequiredService<ApplicationDbContext>();

            Project project = Project.Create(
                $"RP-{Guid.NewGuid():N}",
                "Reporting Project",
                null,
                new DateOnly(2026, 1, 1),
                new DateOnly(2026, 12, 31),
                null,
                null,
                null);

            var first = Activity.Create(
                project.Id,
                $"A-{Guid.NewGuid():N}",
                "Excavation",
                null,
                null,
                null,
                ActivityPriority.Normal,
                new DateOnly(2026, 1, 1),
                new DateOnly(2026, 1, 5));

            first.Start(new DateOnly(2026, 1, 1));
            first.UpdateProgress(50m);

            var second = Activity.Create(
                project.Id,
                $"B-{Guid.NewGuid():N}",
                "Concrete",
                null,
                null,
                null,
                ActivityPriority.Normal,
                new DateOnly(2026, 1, 1),
                new DateOnly(2026, 1, 6));

            second.Complete(new DateOnly(2026, 1, 6));

            dbContext.Projects.Add(project);
            dbContext.Activities.AddRange(first, second);

            await dbContext.SaveChangesAsync(
                TestContext.Current.CancellationToken);

            projectId = project.Id;
        }

        await using AsyncServiceScope reportScope =
            fixture.Factory.Services.CreateAsyncScope();

        IProjectReportingReadService reporting =
            reportScope.ServiceProvider
                .GetRequiredService<IProjectReportingReadService>();

        var report = await reporting.GetActivitiesAsync(
            projectId,
            new DateOnly(2026, 1, 10),
            TestContext.Current.CancellationToken);

        Assert.Equal(2, report.Counts.Total);
        Assert.Equal(1, report.Counts.InProgress);
        Assert.Equal(1, report.Counts.Completed);
        Assert.Equal(1, report.Counts.Overdue);
        Assert.Equal(75m, report.AverageProgressPercentage);
        Assert.Single(report.OverdueActivities);
    }
}
