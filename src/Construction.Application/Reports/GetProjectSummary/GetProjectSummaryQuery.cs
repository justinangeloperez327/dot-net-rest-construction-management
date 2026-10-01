using Construction.Application.Common.Messaging;

namespace Construction.Application.Reports.GetProjectSummary;

public sealed record GetProjectSummaryQuery(
    Guid ProjectId) : IQuery<ProjectSummaryReportResponse>;
