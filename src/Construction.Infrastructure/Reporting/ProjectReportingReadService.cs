using Construction.Application.Abstractions.Reports;
using Construction.Application.Reports;
using Construction.Domain.Activities;
using Construction.Domain.DailyProgress;
using Construction.Domain.Equipment;
using Construction.Domain.Inspections;
using Construction.Domain.Issues;
using Construction.Domain.PurchaseOrders;
using Construction.Domain.PurchaseRequests;
using Construction.Domain.Rfis;
using Construction.Domain.Submittals;
using Construction.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Construction.Infrastructure.Reporting;

public sealed class ProjectReportingReadService(
    ApplicationDbContext dbContext)
    : IProjectReportingReadService
{
    private const int ExceptionListLimit = 50;

    public async Task<ProjectSummaryReportResponse> GetSummaryAsync(
        Guid projectId,
        DateOnly today,
        CancellationToken cancellationToken = default)
    {
        var project = await dbContext.Projects
            .AsNoTracking()
            .Where(value => value.Id == projectId)
            .Select(value => new
            {
                value.Id,
                value.Number,
                value.Name,
                value.Status,
                value.StartDate,
                value.PlannedEndDate,
                value.ActualEndDate
            })
            .SingleAsync(cancellationToken);

        Dictionary<ActivityStatus, int> activityCounts =
            await GetActivityStatusCountsAsync(
                projectId,
                cancellationToken);

        decimal averageProgress =
            await dbContext.Activities
                .AsNoTracking()
                .Where(activity => activity.ProjectId == projectId)
                .Select(activity =>
                    (decimal?)activity.ProgressPercentage)
                .AverageAsync(cancellationToken)
            ?? 0m;

        int overdueActivities = await dbContext.Activities
            .AsNoTracking()
            .CountAsync(
                activity =>
                    activity.ProjectId == projectId
                    && activity.PlannedEndDate != null
                    && activity.PlannedEndDate < today
                    && activity.Status != ActivityStatus.Completed
                    && activity.Status != ActivityStatus.Cancelled,
                cancellationToken);

        Dictionary<DailyProgressStatus, int> dailyCounts =
            await GetDailyProgressStatusCountsAsync(
                projectId,
                null,
                null,
                cancellationToken);

        DateOnly? latestReportDate = await dbContext.DailyProgressReports
            .AsNoTracking()
            .Where(report => report.ProjectId == projectId)
            .Select(report => (DateOnly?)report.ReportDate)
            .MaxAsync(cancellationToken);

        DateOnly? latestApprovedReportDate =
            await dbContext.DailyProgressReports
                .AsNoTracking()
                .Where(report =>
                    report.ProjectId == projectId
                    && report.Status == DailyProgressStatus.Approved)
                .Select(report => (DateOnly?)report.ReportDate)
                .MaxAsync(cancellationToken);

        Dictionary<RfiStatus, int> rfiCounts =
            await GetRfiStatusCountsAsync(
                projectId,
                cancellationToken);

        int overdueRfis = await dbContext.Rfis
            .AsNoTracking()
            .CountAsync(
                rfi =>
                    rfi.ProjectId == projectId
                    && rfi.Status == RfiStatus.Open
                    && rfi.DueDate != null
                    && rfi.DueDate < today,
                cancellationToken);

        Dictionary<SubmittalStatus, int> submittalCounts =
            await GetSubmittalStatusCountsAsync(
                projectId,
                cancellationToken);

        int overdueSubmittals =
            await GetOverdueSubmittalCountAsync(
                projectId,
                today,
                cancellationToken);

        Dictionary<InspectionStatus, int> inspectionCounts =
            await GetInspectionStatusCountsAsync(
                projectId,
                cancellationToken);

        Dictionary<IssueStatus, int> issueCounts =
            await GetIssueStatusCountsAsync(
                projectId,
                cancellationToken);

        int overdueIssues = await dbContext.Issues
            .AsNoTracking()
            .CountAsync(
                issue =>
                    issue.ProjectId == projectId
                    && issue.DueDate != null
                    && issue.DueDate < today
                    && (issue.Status == IssueStatus.Open
                        || issue.Status == IssueStatus.InProgress),
                cancellationToken);

        Dictionary<EquipmentStatus, int> equipmentCounts =
            await GetEquipmentStatusCountsAsync(
                projectId,
                cancellationToken);

        int overdueMaintenance =
            await GetOverdueMaintenanceCountAsync(
                projectId,
                today,
                cancellationToken);

        Dictionary<PurchaseRequestStatus, int> requestCounts =
            await GetPurchaseRequestStatusCountsAsync(
                projectId,
                cancellationToken);

        Dictionary<PurchaseOrderStatus, int> orderCounts =
            await GetPurchaseOrderStatusCountsAsync(
                projectId,
                cancellationToken);

        int overdueOrders =
            await GetOverduePurchaseOrderCountAsync(
                projectId,
                today,
                cancellationToken);

        return new ProjectSummaryReportResponse(
            project.Id,
            project.Number,
            project.Name,
            project.Status,
            project.StartDate,
            project.PlannedEndDate,
            project.ActualEndDate,
            averageProgress,
            BuildActivityCounts(activityCounts, overdueActivities),
            BuildDailyProgressCounts(
                dailyCounts,
                latestReportDate,
                latestApprovedReportDate),
            BuildRfiCounts(rfiCounts, overdueRfis),
            BuildSubmittalCounts(
                submittalCounts,
                overdueSubmittals),
            new QualitySummaryCounts(
                SumCounts(inspectionCounts),
                Count(inspectionCounts, InspectionStatus.Requested),
                Count(inspectionCounts, InspectionStatus.InProgress),
                Count(inspectionCounts, InspectionStatus.Passed),
                Count(inspectionCounts, InspectionStatus.Failed),
                SumCounts(issueCounts),
                Count(issueCounts, IssueStatus.Open),
                Count(issueCounts, IssueStatus.InProgress),
                Count(issueCounts, IssueStatus.PendingVerification),
                Count(issueCounts, IssueStatus.Closed),
                overdueIssues),
            BuildEquipmentCounts(
                equipmentCounts,
                overdueMaintenance),
            new ProcurementSummaryCounts(
                SumCounts(requestCounts),
                Count(
                    requestCounts,
                    PurchaseRequestStatus.Submitted),
                Count(
                    requestCounts,
                    PurchaseRequestStatus.Approved),
                SumCounts(orderCounts),
                Count(orderCounts, PurchaseOrderStatus.Issued),
                Count(
                    orderCounts,
                    PurchaseOrderStatus.PartiallyDelivered),
                Count(orderCounts, PurchaseOrderStatus.Delivered),
                Count(orderCounts, PurchaseOrderStatus.Closed),
                overdueOrders));
    }

    public async Task<ActivityReportResponse> GetActivitiesAsync(
        Guid projectId,
        DateOnly today,
        CancellationToken cancellationToken = default)
    {
        Dictionary<ActivityStatus, int> counts =
            await GetActivityStatusCountsAsync(
                projectId,
                cancellationToken);

        decimal averageProgress =
            await dbContext.Activities
                .AsNoTracking()
                .Where(activity => activity.ProjectId == projectId)
                .Select(activity =>
                    (decimal?)activity.ProgressPercentage)
                .AverageAsync(cancellationToken)
            ?? 0m;

        OverdueActivityReportItem[] overdue =
            await dbContext.Activities
                .AsNoTracking()
                .Where(activity =>
                    activity.ProjectId == projectId
                    && activity.PlannedEndDate != null
                    && activity.PlannedEndDate < today
                    && activity.Status != ActivityStatus.Completed
                    && activity.Status != ActivityStatus.Cancelled)
                .OrderBy(activity => activity.PlannedEndDate)
                .ThenBy(activity => activity.Code)
                .Take(ExceptionListLimit)
                .Select(activity =>
                    new OverdueActivityReportItem(
                        activity.Id,
                        activity.Code,
                        activity.Name,
                        activity.Status,
                        activity.ProgressPercentage,
                        activity.PlannedEndDate!.Value,
                        activity.WorkPackageId,
                        activity.LocationId))
                .ToArrayAsync(cancellationToken);

        int overdueCount = await dbContext.Activities
            .AsNoTracking()
            .CountAsync(
                activity =>
                    activity.ProjectId == projectId
                    && activity.PlannedEndDate != null
                    && activity.PlannedEndDate < today
                    && activity.Status != ActivityStatus.Completed
                    && activity.Status != ActivityStatus.Cancelled,
                cancellationToken);

        return new ActivityReportResponse(
            projectId,
            averageProgress,
            BuildActivityCounts(counts, overdueCount),
            overdue);
    }

    public async Task<DailyProgressReportResponse> GetDailyProgressAsync(
        Guid projectId,
        DateOnly fromDate,
        DateOnly toDate,
        CancellationToken cancellationToken = default)
    {
        Dictionary<DailyProgressStatus, int> counts =
            await GetDailyProgressStatusCountsAsync(
                projectId,
                fromDate,
                toDate,
                cancellationToken);

        IQueryable<DailyProgressReport> reports =
            dbContext.DailyProgressReports
                .AsNoTracking()
                .Where(report =>
                    report.ProjectId == projectId
                    && report.ReportDate >= fromDate
                    && report.ReportDate <= toDate);

        DateOnly? latestReportDate = await reports
            .Select(report => (DateOnly?)report.ReportDate)
            .MaxAsync(cancellationToken);

        DateOnly? latestApprovedReportDate = await reports
            .Where(report =>
                report.Status == DailyProgressStatus.Approved)
            .Select(report => (DateOnly?)report.ReportDate)
            .MaxAsync(cancellationToken);

        IQueryable<Guid> reportIds =
            reports.Select(report => report.Id);

        int totalHeadcount =
            await dbContext.DailyProgressManpower
                .AsNoTracking()
                .Where(item => reportIds.Contains(item.ReportId))
                .Select(item => (int?)item.Headcount)
                .SumAsync(cancellationToken)
            ?? 0;

        decimal manpowerHours =
            await dbContext.DailyProgressManpower
                .AsNoTracking()
                .Where(item => reportIds.Contains(item.ReportId))
                .Select(item => (decimal?)item.TotalHours)
                .SumAsync(cancellationToken)
            ?? 0m;

        int equipmentQuantity =
            await dbContext.DailyProgressEquipment
                .AsNoTracking()
                .Where(item => reportIds.Contains(item.ReportId))
                .Select(item => (int?)item.Quantity)
                .SumAsync(cancellationToken)
            ?? 0;

        decimal equipmentWorkingHours =
            await dbContext.DailyProgressEquipment
                .AsNoTracking()
                .Where(item => reportIds.Contains(item.ReportId))
                .Select(item => (decimal?)item.WorkingHours)
                .SumAsync(cancellationToken)
            ?? 0m;

        decimal equipmentIdleHours =
            await dbContext.DailyProgressEquipment
                .AsNoTracking()
                .Where(item => reportIds.Contains(item.ReportId))
                .Select(item => (decimal?)item.IdleHours)
                .SumAsync(cancellationToken)
            ?? 0m;

        return new DailyProgressReportResponse(
            projectId,
            fromDate,
            toDate,
            BuildDailyProgressCounts(
                counts,
                latestReportDate,
                latestApprovedReportDate),
            totalHeadcount,
            manpowerHours,
            equipmentQuantity,
            equipmentWorkingHours,
            equipmentIdleHours);
    }

    public async Task<QualityReportResponse> GetQualityAsync(
        Guid projectId,
        DateOnly today,
        CancellationToken cancellationToken = default)
    {
        Dictionary<RfiStatus, int> rfiCounts =
            await GetRfiStatusCountsAsync(
                projectId,
                cancellationToken);

        int overdueRfiCount = await dbContext.Rfis
            .AsNoTracking()
            .CountAsync(
                rfi =>
                    rfi.ProjectId == projectId
                    && rfi.Status == RfiStatus.Open
                    && rfi.DueDate != null
                    && rfi.DueDate < today,
                cancellationToken);

        Dictionary<SubmittalStatus, int> submittalCounts =
            await GetSubmittalStatusCountsAsync(
                projectId,
                cancellationToken);

        int overdueSubmittals =
            await GetOverdueSubmittalCountAsync(
                projectId,
                today,
                cancellationToken);

        Dictionary<InspectionStatus, int> inspectionCounts =
            await GetInspectionStatusCountsAsync(
                projectId,
                cancellationToken);

        Dictionary<IssueStatus, int> issueCounts =
            await GetIssueStatusCountsAsync(
                projectId,
                cancellationToken);

        int overdueIssueCount = await dbContext.Issues
            .AsNoTracking()
            .CountAsync(
                issue =>
                    issue.ProjectId == projectId
                    && issue.DueDate != null
                    && issue.DueDate < today
                    && (issue.Status == IssueStatus.Open
                        || issue.Status == IssueStatus.InProgress),
                cancellationToken);

        OverdueRfiReportItem[] overdueRfis =
            await dbContext.Rfis
                .AsNoTracking()
                .Where(rfi =>
                    rfi.ProjectId == projectId
                    && rfi.Status == RfiStatus.Open
                    && rfi.DueDate != null
                    && rfi.DueDate < today)
                .OrderBy(rfi => rfi.DueDate)
                .ThenBy(rfi => rfi.Number)
                .Take(ExceptionListLimit)
                .Select(rfi =>
                    new OverdueRfiReportItem(
                        rfi.Id,
                        rfi.Number,
                        rfi.Subject,
                        rfi.DueDate!.Value,
                        rfi.ResponsibleUserId))
                .ToArrayAsync(cancellationToken);

        OverdueIssueReportItem[] overdueIssues =
            await dbContext.Issues
                .AsNoTracking()
                .Where(issue =>
                    issue.ProjectId == projectId
                    && issue.DueDate != null
                    && issue.DueDate < today
                    && (issue.Status == IssueStatus.Open
                        || issue.Status == IssueStatus.InProgress))
                .OrderBy(issue => issue.DueDate)
                .ThenByDescending(issue => issue.Severity)
                .Take(ExceptionListLimit)
                .Select(issue =>
                    new OverdueIssueReportItem(
                        issue.Id,
                        issue.Number,
                        issue.Title,
                        issue.Severity,
                        issue.DueDate!.Value,
                        issue.ResponsibleUserId))
                .ToArrayAsync(cancellationToken);

        return new QualityReportResponse(
            projectId,
            BuildRfiCounts(rfiCounts, overdueRfiCount),
            BuildSubmittalCounts(
                submittalCounts,
                overdueSubmittals),
            new InspectionReportCounts(
                SumCounts(inspectionCounts),
                Count(inspectionCounts, InspectionStatus.Draft),
                Count(
                    inspectionCounts,
                    InspectionStatus.Requested),
                Count(
                    inspectionCounts,
                    InspectionStatus.InProgress),
                Count(inspectionCounts, InspectionStatus.Passed),
                Count(inspectionCounts, InspectionStatus.Failed),
                Count(
                    inspectionCounts,
                    InspectionStatus.Cancelled)),
            new IssueReportCounts(
                SumCounts(issueCounts),
                Count(issueCounts, IssueStatus.Open),
                Count(issueCounts, IssueStatus.InProgress),
                Count(
                    issueCounts,
                    IssueStatus.PendingVerification),
                Count(issueCounts, IssueStatus.Closed),
                Count(issueCounts, IssueStatus.Cancelled),
                overdueIssueCount),
            overdueRfis,
            overdueIssues);
    }

    public async Task<ProcurementReportResponse> GetProcurementAsync(
        Guid projectId,
        DateOnly today,
        CancellationToken cancellationToken = default)
    {
        Dictionary<EquipmentStatus, int> equipmentCounts =
            await GetEquipmentStatusCountsAsync(
                projectId,
                cancellationToken);

        int overdueMaintenance =
            await GetOverdueMaintenanceCountAsync(
                projectId,
                today,
                cancellationToken);

        Dictionary<PurchaseRequestStatus, int> requestCounts =
            await GetPurchaseRequestStatusCountsAsync(
                projectId,
                cancellationToken);

        Dictionary<PurchaseOrderStatus, int> orderCounts =
            await GetPurchaseOrderStatusCountsAsync(
                projectId,
                cancellationToken);

        int overdueOrders =
            await GetOverduePurchaseOrderCountAsync(
                projectId,
                today,
                cancellationToken);

        var requestAmountsRaw =
            await (
                from item in dbContext.PurchaseRequestItems.AsNoTracking()
                join request in dbContext.PurchaseRequests.AsNoTracking()
                    on item.PurchaseRequestId equals request.Id
                where request.ProjectId == projectId
                    && request.Status != PurchaseRequestStatus.Cancelled
                group new { item, request }
                    by request.CurrencyCode
                into currencyGroup
                orderby currencyGroup.Key
                select new
                {
                    CurrencyCode = currencyGroup.Key,
                    Amount = currencyGroup.Sum(value =>
                        value.item.Quantity
                        * (value.item.EstimatedUnitCost ?? 0m))
                })
                .ToArrayAsync(cancellationToken);

        CurrencyAmountReportItem[] requestAmounts =
            requestAmountsRaw
                .Select(value =>
                    new CurrencyAmountReportItem(
                        value.CurrencyCode,
                        value.Amount))
                .ToArray();

        var orderAmountsRaw =
            await (
                from item in dbContext.PurchaseOrderItems.AsNoTracking()
                join order in dbContext.PurchaseOrders.AsNoTracking()
                    on item.PurchaseOrderId equals order.Id
                where order.ProjectId == projectId
                    && order.Status != PurchaseOrderStatus.Cancelled
                group new { item, order }
                    by order.CurrencyCode
                into currencyGroup
                orderby currencyGroup.Key
                select new
                {
                    CurrencyCode = currencyGroup.Key,
                    OrderedAmount = currencyGroup.Sum(value =>
                        value.item.OrderedQuantity
                        * value.item.UnitPrice),
                    ReceivedAmount = currencyGroup.Sum(value =>
                        value.item.ReceivedQuantity
                        * value.item.UnitPrice)
                })
                .ToArrayAsync(cancellationToken);

        PurchaseOrderCurrencyReportItem[] orderAmounts =
            orderAmountsRaw
                .Select(value =>
                    new PurchaseOrderCurrencyReportItem(
                        value.CurrencyCode,
                        value.OrderedAmount,
                        value.ReceivedAmount))
                .ToArray();

        return new ProcurementReportResponse(
            projectId,
            BuildEquipmentCounts(
                equipmentCounts,
                overdueMaintenance),
            new PurchaseRequestReportCounts(
                SumCounts(requestCounts),
                Count(requestCounts, PurchaseRequestStatus.Draft),
                Count(
                    requestCounts,
                    PurchaseRequestStatus.Submitted),
                Count(
                    requestCounts,
                    PurchaseRequestStatus.Approved),
                Count(
                    requestCounts,
                    PurchaseRequestStatus.Rejected),
                Count(
                    requestCounts,
                    PurchaseRequestStatus.Converted),
                Count(
                    requestCounts,
                    PurchaseRequestStatus.Cancelled)),
            new PurchaseOrderReportCounts(
                SumCounts(orderCounts),
                Count(orderCounts, PurchaseOrderStatus.Draft),
                Count(orderCounts, PurchaseOrderStatus.Issued),
                Count(
                    orderCounts,
                    PurchaseOrderStatus.PartiallyDelivered),
                Count(orderCounts, PurchaseOrderStatus.Delivered),
                Count(orderCounts, PurchaseOrderStatus.Closed),
                Count(orderCounts, PurchaseOrderStatus.Cancelled),
                overdueOrders),
            requestAmounts,
            orderAmounts);
    }

    private async Task<Dictionary<ActivityStatus, int>>
        GetActivityStatusCountsAsync(
            Guid projectId,
            CancellationToken cancellationToken) =>
        await dbContext.Activities
            .AsNoTracking()
            .Where(activity => activity.ProjectId == projectId)
            .GroupBy(activity => activity.Status)
            .Select(group => new
            {
                Status = group.Key,
                Count = group.Count()
            })
            .ToDictionaryAsync(
                value => value.Status,
                value => value.Count,
                cancellationToken);

    private async Task<Dictionary<DailyProgressStatus, int>>
        GetDailyProgressStatusCountsAsync(
            Guid projectId,
            DateOnly? fromDate,
            DateOnly? toDate,
            CancellationToken cancellationToken)
    {
        IQueryable<DailyProgressReport> query =
            dbContext.DailyProgressReports
                .AsNoTracking()
                .Where(report => report.ProjectId == projectId);

        if (fromDate is DateOnly start)
        {
            query = query.Where(report =>
                report.ReportDate >= start);
        }

        if (toDate is DateOnly end)
        {
            query = query.Where(report =>
                report.ReportDate <= end);
        }

        return await query
            .GroupBy(report => report.Status)
            .Select(group => new
            {
                Status = group.Key,
                Count = group.Count()
            })
            .ToDictionaryAsync(
                value => value.Status,
                value => value.Count,
                cancellationToken);
    }

    private async Task<Dictionary<RfiStatus, int>>
        GetRfiStatusCountsAsync(
            Guid projectId,
            CancellationToken cancellationToken) =>
        await dbContext.Rfis
            .AsNoTracking()
            .Where(rfi => rfi.ProjectId == projectId)
            .GroupBy(rfi => rfi.Status)
            .Select(group => new
            {
                Status = group.Key,
                Count = group.Count()
            })
            .ToDictionaryAsync(
                value => value.Status,
                value => value.Count,
                cancellationToken);

    private async Task<Dictionary<SubmittalStatus, int>>
        GetSubmittalStatusCountsAsync(
            Guid projectId,
            CancellationToken cancellationToken) =>
        await dbContext.Submittals
            .AsNoTracking()
            .Where(submittal =>
                submittal.ProjectId == projectId)
            .GroupBy(submittal => submittal.Status)
            .Select(group => new
            {
                Status = group.Key,
                Count = group.Count()
            })
            .ToDictionaryAsync(
                value => value.Status,
                value => value.Count,
                cancellationToken);

    private async Task<Dictionary<InspectionStatus, int>>
        GetInspectionStatusCountsAsync(
            Guid projectId,
            CancellationToken cancellationToken) =>
        await dbContext.Inspections
            .AsNoTracking()
            .Where(inspection =>
                inspection.ProjectId == projectId)
            .GroupBy(inspection => inspection.Status)
            .Select(group => new
            {
                Status = group.Key,
                Count = group.Count()
            })
            .ToDictionaryAsync(
                value => value.Status,
                value => value.Count,
                cancellationToken);

    private async Task<Dictionary<IssueStatus, int>>
        GetIssueStatusCountsAsync(
            Guid projectId,
            CancellationToken cancellationToken) =>
        await dbContext.Issues
            .AsNoTracking()
            .Where(issue => issue.ProjectId == projectId)
            .GroupBy(issue => issue.Status)
            .Select(group => new
            {
                Status = group.Key,
                Count = group.Count()
            })
            .ToDictionaryAsync(
                value => value.Status,
                value => value.Count,
                cancellationToken);

    private async Task<Dictionary<EquipmentStatus, int>>
        GetEquipmentStatusCountsAsync(
            Guid projectId,
            CancellationToken cancellationToken) =>
        await dbContext.Equipment
            .AsNoTracking()
            .Where(equipment =>
                equipment.ProjectId == projectId)
            .GroupBy(equipment => equipment.Status)
            .Select(group => new
            {
                Status = group.Key,
                Count = group.Count()
            })
            .ToDictionaryAsync(
                value => value.Status,
                value => value.Count,
                cancellationToken);

    private async Task<Dictionary<PurchaseRequestStatus, int>>
        GetPurchaseRequestStatusCountsAsync(
            Guid projectId,
            CancellationToken cancellationToken) =>
        await dbContext.PurchaseRequests
            .AsNoTracking()
            .Where(request =>
                request.ProjectId == projectId)
            .GroupBy(request => request.Status)
            .Select(group => new
            {
                Status = group.Key,
                Count = group.Count()
            })
            .ToDictionaryAsync(
                value => value.Status,
                value => value.Count,
                cancellationToken);

    private async Task<Dictionary<PurchaseOrderStatus, int>>
        GetPurchaseOrderStatusCountsAsync(
            Guid projectId,
            CancellationToken cancellationToken) =>
        await dbContext.PurchaseOrders
            .AsNoTracking()
            .Where(order =>
                order.ProjectId == projectId)
            .GroupBy(order => order.Status)
            .Select(group => new
            {
                Status = group.Key,
                Count = group.Count()
            })
            .ToDictionaryAsync(
                value => value.Status,
                value => value.Count,
                cancellationToken);

    private async Task<int> GetOverdueSubmittalCountAsync(
        Guid projectId,
        DateOnly today,
        CancellationToken cancellationToken) =>
        await (
            from revision in dbContext.SubmittalRevisions.AsNoTracking()
            join submittal in dbContext.Submittals.AsNoTracking()
                on revision.SubmittalId equals submittal.Id
            where submittal.ProjectId == projectId
                && revision.IsCurrent
                && revision.ReviewDueDate != null
                && revision.ReviewDueDate < today
                && (submittal.Status == SubmittalStatus.Submitted
                    || submittal.Status == SubmittalStatus.UnderReview)
            select revision.Id)
            .CountAsync(cancellationToken);

    private async Task<int> GetOverdueMaintenanceCountAsync(
        Guid projectId,
        DateOnly today,
        CancellationToken cancellationToken) =>
        await (
            from maintenance in dbContext.EquipmentMaintenanceRecords
                .AsNoTracking()
            join equipment in dbContext.Equipment.AsNoTracking()
                on maintenance.EquipmentId equals equipment.Id
            where equipment.ProjectId == projectId
                && maintenance.CompletedDate == null
                && maintenance.ScheduledDate < today
            select maintenance.Id)
            .CountAsync(cancellationToken);

    private Task<int> GetOverduePurchaseOrderCountAsync(
        Guid projectId,
        DateOnly today,
        CancellationToken cancellationToken) =>
        dbContext.PurchaseOrders
            .AsNoTracking()
            .CountAsync(
                order =>
                    order.ProjectId == projectId
                    && order.ExpectedDeliveryDate != null
                    && order.ExpectedDeliveryDate < today
                    && (order.Status == PurchaseOrderStatus.Issued
                        || order.Status
                            == PurchaseOrderStatus.PartiallyDelivered),
                cancellationToken);

    private static ActivitySummaryCounts BuildActivityCounts(
        IReadOnlyDictionary<ActivityStatus, int> counts,
        int overdue) =>
        new(
            SumCounts(counts),
            Count(counts, ActivityStatus.NotStarted),
            Count(counts, ActivityStatus.InProgress),
            Count(counts, ActivityStatus.OnHold),
            Count(counts, ActivityStatus.Completed),
            Count(counts, ActivityStatus.Cancelled),
            overdue);

    private static DailyProgressSummaryCounts BuildDailyProgressCounts(
        IReadOnlyDictionary<DailyProgressStatus, int> counts,
        DateOnly? latestReportDate,
        DateOnly? latestApprovedReportDate) =>
        new(
            SumCounts(counts),
            Count(counts, DailyProgressStatus.Draft),
            Count(counts, DailyProgressStatus.Submitted),
            Count(counts, DailyProgressStatus.Approved),
            Count(counts, DailyProgressStatus.Rejected),
            latestReportDate,
            latestApprovedReportDate);

    private static RfiSummaryCounts BuildRfiCounts(
        IReadOnlyDictionary<RfiStatus, int> counts,
        int overdueOpen) =>
        new(
            SumCounts(counts),
            Count(counts, RfiStatus.Draft),
            Count(counts, RfiStatus.Open),
            Count(counts, RfiStatus.Answered),
            Count(counts, RfiStatus.Closed),
            Count(counts, RfiStatus.Cancelled),
            overdueOpen);

    private static SubmittalSummaryCounts BuildSubmittalCounts(
        IReadOnlyDictionary<SubmittalStatus, int> counts,
        int overduePending) =>
        new(
            SumCounts(counts),
            Count(counts, SubmittalStatus.Draft),
            Count(counts, SubmittalStatus.Submitted),
            Count(counts, SubmittalStatus.UnderReview),
            Count(counts, SubmittalStatus.Approved),
            Count(
                counts,
                SubmittalStatus.ApprovedWithComments),
            Count(counts, SubmittalStatus.Rejected),
            Count(counts, SubmittalStatus.Closed),
            Count(counts, SubmittalStatus.Cancelled),
            overduePending);

    private static EquipmentSummaryCounts BuildEquipmentCounts(
        IReadOnlyDictionary<EquipmentStatus, int> counts,
        int overdueMaintenance) =>
        new(
            SumCounts(counts),
            Count(counts, EquipmentStatus.Available),
            Count(counts, EquipmentStatus.InUse),
            Count(counts, EquipmentStatus.Maintenance),
            Count(counts, EquipmentStatus.OutOfService),
            Count(counts, EquipmentStatus.Retired),
            overdueMaintenance);

    private static int Count<TStatus>(
        IReadOnlyDictionary<TStatus, int> counts,
        TStatus status)
        where TStatus : struct, Enum =>
        counts.TryGetValue(status, out int value)
            ? value
            : 0;

    private static int SumCounts<TStatus>(
        IReadOnlyDictionary<TStatus, int> counts)
        where TStatus : struct, Enum =>
        counts.Values.Sum();
}
