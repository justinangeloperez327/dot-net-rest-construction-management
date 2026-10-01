using Construction.Application.Common.Messaging;

namespace Construction.Application.Reports.GetActivityReport;

public sealed record GetActivityReportQuery(
    Guid ProjectId) : IQuery<ActivityReportResponse>;
