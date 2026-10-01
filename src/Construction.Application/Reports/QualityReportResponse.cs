using Construction.Domain.Issues;

namespace Construction.Application.Reports;

public sealed record QualityReportResponse(
    Guid ProjectId,
    RfiSummaryCounts Rfis,
    SubmittalSummaryCounts Submittals,
    InspectionReportCounts Inspections,
    IssueReportCounts Issues,
    IReadOnlyCollection<OverdueRfiReportItem> OverdueRfis,
    IReadOnlyCollection<OverdueIssueReportItem> OverdueIssues);

public sealed record InspectionReportCounts(
    int Total,
    int Draft,
    int Requested,
    int InProgress,
    int Passed,
    int Failed,
    int Cancelled);

public sealed record IssueReportCounts(
    int Total,
    int Open,
    int InProgress,
    int PendingVerification,
    int Closed,
    int Cancelled,
    int Overdue);

public sealed record OverdueRfiReportItem(
    Guid Id,
    string Number,
    string Subject,
    DateOnly DueDate,
    Guid? ResponsibleUserId);

public sealed record OverdueIssueReportItem(
    Guid Id,
    string Number,
    string Title,
    IssueSeverity Severity,
    DateOnly DueDate,
    Guid? ResponsibleUserId);
