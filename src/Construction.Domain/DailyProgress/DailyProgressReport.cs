using Construction.Domain.Common;

namespace Construction.Domain.DailyProgress;

public sealed class DailyProgressReport : AuditableAggregateRoot<Guid>
{
    private readonly List<DailyProgressActivity> _activities = [];
    private readonly List<DailyProgressManpower> _manpower = [];
    private readonly List<DailyProgressEquipment> _equipment = [];

    private DailyProgressReport()
        : base(Guid.Empty)
    {
    }

    private DailyProgressReport(
        Guid id,
        Guid projectId,
        DateOnly reportDate,
        Guid createdByUserId,
        WeatherCondition weather,
        decimal? temperatureCelsius,
        string? workSummary,
        string? remarks)
        : base(id)
    {
        ProjectId = projectId;
        ReportDate = reportDate;
        CreatedByUserId = createdByUserId;
        Status = DailyProgressStatus.Draft;

        UpdateHeader(
            weather,
            temperatureCelsius,
            workSummary,
            remarks);
    }

    public Guid ProjectId { get; private set; }

    public DateOnly ReportDate { get; private set; }

    public Guid CreatedByUserId { get; private set; }

    public WeatherCondition Weather { get; private set; }

    public decimal? TemperatureCelsius { get; private set; }

    public string? WorkSummary { get; private set; }

    public string? Remarks { get; private set; }

    public DailyProgressStatus Status { get; private set; }

    public Guid? SubmittedByUserId { get; private set; }

    public DateTimeOffset? SubmittedAtUtc { get; private set; }

    public Guid? ReviewedByUserId { get; private set; }

    public DateTimeOffset? ReviewedAtUtc { get; private set; }

    public string? RejectionReason { get; private set; }

    public IReadOnlyCollection<DailyProgressActivity> Activities => _activities;

    public IReadOnlyCollection<DailyProgressManpower> Manpower => _manpower;

    public IReadOnlyCollection<DailyProgressEquipment> Equipment => _equipment;

    public static DailyProgressReport Create(
        Guid projectId,
        DateOnly reportDate,
        Guid createdByUserId,
        WeatherCondition weather,
        decimal? temperatureCelsius,
        string? workSummary,
        string? remarks)
    {
        if (projectId == Guid.Empty)
        {
            throw new DomainException(
                "Project identifier is required.");
        }

        if (createdByUserId == Guid.Empty)
        {
            throw new DomainException(
                "Report creator identifier is required.");
        }

        return new DailyProgressReport(
            Guid.CreateVersion7(),
            projectId,
            reportDate,
            createdByUserId,
            weather,
            temperatureCelsius,
            workSummary,
            remarks);
    }

    public void UpdateHeader(
        WeatherCondition weather,
        decimal? temperatureCelsius,
        string? workSummary,
        string? remarks)
    {
        PrepareForEdit();

        if (temperatureCelsius is < -80 or > 80)
        {
            throw new DomainException(
                "Temperature must be between -80 and 80 degrees Celsius.");
        }

        if (workSummary?.Trim().Length > 5000)
        {
            throw new DomainException(
                "Daily work summary cannot exceed 5000 characters.");
        }

        if (remarks?.Trim().Length > 5000)
        {
            throw new DomainException(
                "Daily progress remarks cannot exceed 5000 characters.");
        }

        Weather = weather;
        TemperatureCelsius = temperatureCelsius;
        WorkSummary = NormalizeOptional(workSummary);
        Remarks = NormalizeOptional(remarks);
    }

    public void ReplaceActivities(
        IEnumerable<DailyProgressActivityInput> activities)
    {
        ArgumentNullException.ThrowIfNull(activities);
        PrepareForEdit();

        DailyProgressActivityInput[] items = [.. activities];

        if (items
            .GroupBy(item => item.ActivityId)
            .Any(group => group.Count() > 1))
        {
            throw new DomainException(
                "An activity can only appear once in a daily progress report.");
        }

        _activities.Clear();

        foreach (DailyProgressActivityInput item in items)
        {
            _activities.Add(new DailyProgressActivity(
                Guid.CreateVersion7(),
                Id,
                item.ActivityId,
                item.WorkDescription,
                item.ReportedProgressPercentage,
                item.QuantityCompleted,
                item.Unit));
        }
    }

    public void ReplaceManpower(
        IEnumerable<DailyProgressManpowerInput> manpower)
    {
        ArgumentNullException.ThrowIfNull(manpower);
        PrepareForEdit();

        _manpower.Clear();

        foreach (DailyProgressManpowerInput item in manpower)
        {
            _manpower.Add(new DailyProgressManpower(
                Guid.CreateVersion7(),
                Id,
                item.CompanyId,
                item.Trade,
                item.Headcount,
                item.TotalHours));
        }
    }

    public void ReplaceEquipment(
        IEnumerable<DailyProgressEquipmentInput> equipment)
    {
        ArgumentNullException.ThrowIfNull(equipment);
        PrepareForEdit();

        _equipment.Clear();

        foreach (DailyProgressEquipmentInput item in equipment)
        {
            _equipment.Add(new DailyProgressEquipment(
                Guid.CreateVersion7(),
                Id,
                item.Description,
                item.Quantity,
                item.WorkingHours,
                item.IdleHours));
        }
    }

    public void Submit(
        Guid submittedByUserId,
        DateTimeOffset submittedAtUtc)
    {
        if (Status != DailyProgressStatus.Draft)
        {
            throw new DomainException(
                "Only draft daily progress reports can be submitted.");
        }

        if (submittedByUserId == Guid.Empty)
        {
            throw new DomainException(
                "Submitter identifier is required.");
        }

        Status = DailyProgressStatus.Submitted;
        SubmittedByUserId = submittedByUserId;
        SubmittedAtUtc = submittedAtUtc;
        ReviewedByUserId = null;
        ReviewedAtUtc = null;
        RejectionReason = null;
    }

    public void Approve(
        Guid reviewerUserId,
        DateTimeOffset reviewedAtUtc)
    {
        EnsureSubmitted();

        if (reviewerUserId == Guid.Empty)
        {
            throw new DomainException(
                "Reviewer identifier is required.");
        }

        Status = DailyProgressStatus.Approved;
        ReviewedByUserId = reviewerUserId;
        ReviewedAtUtc = reviewedAtUtc;
        RejectionReason = null;
    }

    public void Reject(
        Guid reviewerUserId,
        DateTimeOffset reviewedAtUtc,
        string reason)
    {
        EnsureSubmitted();

        if (reviewerUserId == Guid.Empty)
        {
            throw new DomainException(
                "Reviewer identifier is required.");
        }

        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new DomainException(
                "A rejection reason is required.");
        }

        if (reason.Trim().Length > 2000)
        {
            throw new DomainException(
                "Rejection reason cannot exceed 2000 characters.");
        }

        Status = DailyProgressStatus.Rejected;
        ReviewedByUserId = reviewerUserId;
        ReviewedAtUtc = reviewedAtUtc;
        RejectionReason = reason.Trim();
    }

    public bool CanDelete() =>
        Status is DailyProgressStatus.Draft
            or DailyProgressStatus.Rejected;

    private void PrepareForEdit()
    {
        if (Status == DailyProgressStatus.Submitted)
        {
            throw new DomainException(
                "Submitted daily progress reports cannot be edited.");
        }

        if (Status == DailyProgressStatus.Approved)
        {
            throw new DomainException(
                "Approved daily progress reports are immutable.");
        }

        if (Status == DailyProgressStatus.Rejected)
        {
            Status = DailyProgressStatus.Draft;
            SubmittedByUserId = null;
            SubmittedAtUtc = null;
            ReviewedByUserId = null;
            ReviewedAtUtc = null;
            RejectionReason = null;
        }
    }

    private void EnsureSubmitted()
    {
        if (Status != DailyProgressStatus.Submitted)
        {
            throw new DomainException(
                "Only submitted daily progress reports can be reviewed.");
        }
    }

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
}

public sealed record DailyProgressActivityInput(
    Guid ActivityId,
    string WorkDescription,
    decimal ReportedProgressPercentage,
    decimal? QuantityCompleted,
    string? Unit);

public sealed record DailyProgressManpowerInput(
    Guid? CompanyId,
    string Trade,
    int Headcount,
    decimal TotalHours);

public sealed record DailyProgressEquipmentInput(
    string Description,
    int Quantity,
    decimal WorkingHours,
    decimal IdleHours);
