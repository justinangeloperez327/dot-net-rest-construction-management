using Construction.Domain.Common;
using Construction.Domain.Projects;
using Xunit;

namespace Construction.Domain.Tests.Projects;

public sealed class ProjectTests
{
    [Fact]
    public void Complete_RejectsCompletionBeforeStartDate()
    {
        Project project = Project.Create(
            "PRJ-001",
            "Test Project",
            null,
            new DateOnly(2026, 1, 10),
            new DateOnly(2026, 12, 31),
            null,
            null,
            null);

        Assert.Throws<DomainException>(
            () => project.Complete(
                new DateOnly(2026, 1, 9)));
    }
}
