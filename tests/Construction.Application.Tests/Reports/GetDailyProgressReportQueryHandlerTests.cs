using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Abstractions.Reports;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Pagination;
using Construction.Application.Reports;
using Construction.Application.Reports.GetDailyProgressReport;
using Construction.Domain.Audit;
using Construction.Domain.Projects;
using Xunit;

namespace Construction.Application.Tests.Reports;

public sealed class GetDailyProgressReportQueryHandlerTests
{
    [Fact]
    public async Task HandleAsync_DefaultsToMostRecentThirtyDays()
    {
        Project project = CreateProject();
        var reporting = new ReportingReadServiceFake();

        var handler = new GetDailyProgressReportQueryHandler(
            new ProjectRepositoryFake(project),
            reporting,
            new CurrentUserFake(
                true,
                Guid.NewGuid(),
                new HashSet<string>(StringComparer.Ordinal)
                {
                    Permissions.Reports.View
                }),
            new ProjectAccessServiceFake(true),
            new FixedTimeProvider(
                new DateTimeOffset(
                    2026,
                    1,
                    30,
                    12,
                    0,
                    0,
                    TimeSpan.Zero)));

        var result = await handler.HandleAsync(
            new GetDailyProgressReportQuery(
                project.Id,
                null,
                null),
            TestContext.Current.CancellationToken);

        Assert.True(result.IsSuccess);
        Assert.Equal(
            new DateOnly(2026, 1, 1),
            reporting.LastFromDate);
        Assert.Equal(
            new DateOnly(2026, 1, 30),
            reporting.LastToDate);
    }

    [Fact]
    public async Task HandleAsync_RejectsInvalidDateRangeBeforeReportingQuery()
    {
        Project project = CreateProject();
        var reporting = new ReportingReadServiceFake();

        var handler = new GetDailyProgressReportQueryHandler(
            new ProjectRepositoryFake(project),
            reporting,
            new CurrentUserFake(
                true,
                Guid.NewGuid(),
                new HashSet<string>(StringComparer.Ordinal)
                {
                    Permissions.Reports.View
                }),
            new ProjectAccessServiceFake(true),
            TimeProvider.System);

        var result = await handler.HandleAsync(
            new GetDailyProgressReportQuery(
                project.Id,
                new DateOnly(2026, 2, 1),
                new DateOnly(2026, 1, 1)),
            TestContext.Current.CancellationToken);

        Assert.True(result.IsFailure);
        Assert.Equal(
            "Reports.InvalidDateRange",
            Assert.Single(result.Errors).Code);
        Assert.Equal(0, reporting.DailyProgressCallCount);
    }

    [Fact]
    public async Task HandleAsync_DeniesUserWithoutProjectAccess()
    {
        Project project = CreateProject();

        var handler = new GetDailyProgressReportQueryHandler(
            new ProjectRepositoryFake(project),
            new ReportingReadServiceFake(),
            new CurrentUserFake(
                true,
                Guid.NewGuid(),
                new HashSet<string>(StringComparer.Ordinal)
                {
                    Permissions.Reports.View
                }),
            new ProjectAccessServiceFake(false),
            TimeProvider.System);

        var result = await handler.HandleAsync(
            new GetDailyProgressReportQuery(
                project.Id,
                null,
                null),
            TestContext.Current.CancellationToken);

        Assert.True(result.IsFailure);
        Assert.Equal(
            "Authorization.ProjectAccessDenied",
            Assert.Single(result.Errors).Code);
    }

    private static Project CreateProject() =>
        Project.Create(
            "PRJ-TEST",
            "Test Project",
            null,
            new DateOnly(2026, 1, 1),
            new DateOnly(2026, 12, 31),
            null,
            null,
            null);

    private sealed class CurrentUserFake(
        bool isAuthenticated,
        Guid? userId,
        IReadOnlySet<string> permissions)
        : ICurrentUser
    {
        public bool IsAuthenticated { get; } = isAuthenticated;
        public Guid? UserId { get; } = userId;
        public IReadOnlySet<string> Roles { get; } =
            new HashSet<string>(StringComparer.Ordinal);
        public IReadOnlySet<string> Permissions { get; } = permissions;
    }

    private sealed class ProjectAccessServiceFake(bool allowed)
        : IProjectAccessService
    {
        public Task<bool> HasProjectAccessAsync(
            Guid userId,
            Guid projectId,
            string permission,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(allowed);
    }

    private sealed class ProjectRepositoryFake(Project project)
        : IProjectRepository
    {
        public Task<Project?> GetAsync(
            Guid id,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<Project?>(
                id == project.Id ? project : null);

        public Task<bool> ExistsByNumberAsync(
            string normalizedNumber,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(false);

        public Task<(IReadOnlyCollection<Project> Items, long TotalCount)> GetPageAsync(
            PageRequest page,
            Guid? userId = null,
            bool includeAllProjects = false,
            CancellationToken cancellationToken = default) =>
            Task.FromResult((
                (IReadOnlyCollection<Project>)[project],
                1L));

        public void Add(Project value)
        {
        }
    }

    private sealed class ReportingReadServiceFake
        : IProjectReportingReadService
    {
        public DateOnly? LastFromDate { get; private set; }
        public DateOnly? LastToDate { get; private set; }
        public int DailyProgressCallCount { get; private set; }

        public Task<ProjectSummaryReportResponse> GetSummaryAsync(
            Guid projectId,
            DateOnly today,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<ActivityReportResponse> GetActivitiesAsync(
            Guid projectId,
            DateOnly today,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<DailyProgressReportResponse> GetDailyProgressAsync(
            Guid projectId,
            DateOnly fromDate,
            DateOnly toDate,
            CancellationToken cancellationToken = default)
        {
            DailyProgressCallCount++;
            LastFromDate = fromDate;
            LastToDate = toDate;

            return Task.FromResult(
                new DailyProgressReportResponse(
                    projectId,
                    fromDate,
                    toDate,
                    new DailyProgressSummaryCounts(
                        0,
                        0,
                        0,
                        0,
                        0,
                        null,
                        null),
                    0,
                    0m,
                    0,
                    0m,
                    0m));
        }

        public Task<QualityReportResponse> GetQualityAsync(
            Guid projectId,
            DateOnly today,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<ProcurementReportResponse> GetProcurementAsync(
            Guid projectId,
            DateOnly today,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();
    }

    private sealed class FixedTimeProvider(
        DateTimeOffset utcNow)
        : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => utcNow;
    }
}
