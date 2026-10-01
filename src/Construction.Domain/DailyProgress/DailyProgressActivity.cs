using Construction.Domain.Common;

namespace Construction.Domain.DailyProgress;

public sealed class DailyProgressActivity : AuditableEntity<Guid>
{
    private DailyProgressActivity()
        : base(Guid.Empty)
    {
    }

    internal DailyProgressActivity(
        Guid id,
        Guid reportId,
        Guid activityId,
        string workDescription,
        decimal reportedProgressPercentage,
        decimal? quantityCompleted,
        string? unit)
        : base(id)
    {
        ReportId = reportId;
        ActivityId = activityId;
        Update(
            workDescription,
            reportedProgressPercentage,
            quantityCompleted,
            unit);
    }

    public Guid ReportId { get; private set; }

    public Guid ActivityId { get; private set; }

    public string WorkDescription { get; private set; } = string.Empty;

    public decimal ReportedProgressPercentage { get; private set; }

    public decimal? QuantityCompleted { get; private set; }

    public string? Unit { get; private set; }

    internal void Update(
        string workDescription,
        decimal reportedProgressPercentage,
        decimal? quantityCompleted,
        string? unit)
    {
        if (string.IsNullOrWhiteSpace(workDescription))
        {
            throw new DomainException(
                "Daily activity work description is required.");
        }

        if (workDescription.Trim().Length > 2000)
        {
            throw new DomainException(
                "Daily activity work description cannot exceed 2000 characters.");
        }

        if (reportedProgressPercentage is < 0 or > 100)
        {
            throw new DomainException(
                "Reported activity progress must be between 0 and 100.");
        }

        if (quantityCompleted < 0)
        {
            throw new DomainException(
                "Completed quantity cannot be negative.");
        }

        if (unit?.Trim().Length > 50)
        {
            throw new DomainException(
                "Activity quantity unit cannot exceed 50 characters.");
        }

        WorkDescription = workDescription.Trim();
        ReportedProgressPercentage = reportedProgressPercentage;
        QuantityCompleted = quantityCompleted;
        Unit = string.IsNullOrWhiteSpace(unit)
            ? null
            : unit.Trim();
    }
}
