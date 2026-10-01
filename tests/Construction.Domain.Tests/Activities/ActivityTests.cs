using Construction.Domain.Activities;
using Construction.Domain.Common;
using Xunit;

namespace Construction.Domain.Tests.Activities;

public sealed class ActivityTests
{
    [Fact]
    public void Complete_SetsCompletedStateAndFullProgress()
    {
        Activity activity = Activity.Create(
            Guid.NewGuid(),
            "ACT-001",
            "Excavation",
            null,
            null,
            null,
            ActivityPriority.Normal,
            new DateOnly(2026, 1, 1),
            new DateOnly(2026, 1, 31));

        activity.Start(new DateOnly(2026, 1, 2));
        activity.UpdateProgress(45m);
        activity.Complete(new DateOnly(2026, 1, 25));

        Assert.Equal(ActivityStatus.Completed, activity.Status);
        Assert.Equal(100m, activity.ProgressPercentage);
        Assert.Equal(new DateOnly(2026, 1, 2), activity.ActualStartDate);
        Assert.Equal(new DateOnly(2026, 1, 25), activity.ActualEndDate);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void UpdateProgress_RejectsValuesOutsidePercentageRange(
        decimal percentage)
    {
        Activity activity = Activity.Create(
            Guid.NewGuid(),
            "ACT-002",
            "Concrete",
            null,
            null,
            null,
            ActivityPriority.Normal,
            null,
            null);

        Assert.Throws<DomainException>(
            () => activity.UpdateProgress(percentage));
    }
}
