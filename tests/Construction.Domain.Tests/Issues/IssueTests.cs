using Construction.Domain.Common;
using Construction.Domain.Issues;
using Xunit;

namespace Construction.Domain.Tests.Issues;

public sealed class IssueTests
{
    [Fact]
    public void SubmitForVerification_RequiresCorrectiveActionsToBeResolved()
    {
        Guid actorUserId = Guid.NewGuid();
        DateTimeOffset now =
            new(2026, 1, 1, 8, 0, 0, TimeSpan.Zero);

        Issue issue = Issue.Create(
            Guid.NewGuid(),
            "ISS-001",
            "Honeycombing",
            "Concrete defect",
            IssueType.Defect,
            IssueSeverity.High,
            null,
            null,
            actorUserId,
            new DateOnly(2026, 1, 10),
            actorUserId,
            now);

        CorrectiveAction action = issue.AddCorrectiveAction(
            "Repair affected concrete",
            actorUserId,
            new DateOnly(2026, 1, 5),
            actorUserId,
            now);

        Assert.Throws<DomainException>(
            () => issue.SubmitForVerification(
                "Repair complete",
                actorUserId,
                now.AddHours(1)));

        issue.UpdateCorrectiveAction(
            action.Id,
            action.Description,
            actorUserId,
            action.DueDate,
            CorrectiveActionStatus.Completed,
            "Repair completed",
            actorUserId,
            now.AddHours(2));

        issue.SubmitForVerification(
            "Repair complete",
            actorUserId,
            now.AddHours(3));

        Assert.Equal(
            IssueStatus.PendingVerification,
            issue.Status);
    }
}
