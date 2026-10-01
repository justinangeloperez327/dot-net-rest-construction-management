namespace Construction.Application.Reports;

public sealed record ProcurementReportResponse(
    Guid ProjectId,
    EquipmentSummaryCounts Equipment,
    PurchaseRequestReportCounts PurchaseRequests,
    PurchaseOrderReportCounts PurchaseOrders,
    IReadOnlyCollection<CurrencyAmountReportItem> PurchaseRequestEstimatedTotals,
    IReadOnlyCollection<PurchaseOrderCurrencyReportItem> PurchaseOrderTotals);

public sealed record PurchaseRequestReportCounts(
    int Total,
    int Draft,
    int Submitted,
    int Approved,
    int Rejected,
    int Converted,
    int Cancelled);

public sealed record PurchaseOrderReportCounts(
    int Total,
    int Draft,
    int Issued,
    int PartiallyDelivered,
    int Delivered,
    int Closed,
    int Cancelled,
    int Overdue);

public sealed record CurrencyAmountReportItem(
    string CurrencyCode,
    decimal Amount);

public sealed record PurchaseOrderCurrencyReportItem(
    string CurrencyCode,
    decimal OrderedAmount,
    decimal ReceivedAmount);
