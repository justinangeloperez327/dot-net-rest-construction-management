using Construction.Application.Common.Messaging;

namespace Construction.Application.Reports.GetQualityReport;

public sealed record GetQualityReportQuery(
    Guid ProjectId) : IQuery<QualityReportResponse>;
