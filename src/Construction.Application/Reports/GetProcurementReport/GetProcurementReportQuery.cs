using Construction.Application.Common.Messaging;

namespace Construction.Application.Reports.GetProcurementReport;

public sealed record GetProcurementReportQuery(
    Guid ProjectId) : IQuery<ProcurementReportResponse>;
