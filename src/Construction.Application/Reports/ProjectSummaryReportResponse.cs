using Construction.Domain.Projects;

namespace Construction.Application.Reports;

public sealed record ProjectSummaryReportResponse(
    Guid ProjectId,
    string ProjectNumber,
    string ProjectName,
    ProjectStatus ProjectStatus,
    DateOnly? StartDate,
    DateOnly? PlannedEndDate,
    DateOnly? ActualEndDate,
    decimal AverageActivityProgressPercentage,
    ActivitySummaryCounts Activities,
    DailyProgressSummaryCounts DailyProgress,
    RfiSummaryCounts Rfis,
    SubmittalSummaryCounts Submittals,
    QualitySummaryCounts Quality,
    EquipmentSummaryCounts Equipment,
    ProcurementSummaryCounts Procurement);

public sealed record ActivitySummaryCounts(
    int Total,
    int NotStarted,
    int InProgress,
    int OnHold,
    int Completed,
    int Cancelled,
    int Overdue);

public sealed record DailyProgressSummaryCounts(
    int Total,
    int Draft,
    int Submitted,
    int Approved,
    int Rejected,
    DateOnly? LatestReportDate,
    DateOnly? LatestApprovedReportDate);

public sealed record RfiSummaryCounts(
    int Total,
    int Draft,
    int Open,
    int Answered,
    int Closed,
    int Cancelled,
    int OverdueOpen);

public sealed record SubmittalSummaryCounts(
    int Total,
    int Draft,
    int Submitted,
    int UnderReview,
    int Approved,
    int ApprovedWithComments,
    int Rejected,
    int Closed,
    int Cancelled);

public sealed record QualitySummaryCounts(
    int InspectionsTotal,
    int InspectionsRequested,
    int InspectionsInProgress,
    int InspectionsPassed,
    int InspectionsFailed,
    int IssuesTotal,
    int IssuesOpen,
    int IssuesInProgress,
    int IssuesPendingVerification,
    int IssuesClosed,
    int OverdueIssues);

public sealed record EquipmentSummaryCounts(
    int Total,
    int Available,
    int InUse,
    int Maintenance,
    int OutOfService,
    int Retired,
    int OverdueMaintenance);

public sealed record ProcurementSummaryCounts(
    int PurchaseRequestsTotal,
    int PurchaseRequestsSubmitted,
    int PurchaseRequestsApproved,
    int PurchaseOrdersTotal,
    int PurchaseOrdersIssued,
    int PurchaseOrdersPartiallyDelivered,
    int PurchaseOrdersDelivered,
    int PurchaseOrdersClosed,
    int OverduePurchaseOrders);
