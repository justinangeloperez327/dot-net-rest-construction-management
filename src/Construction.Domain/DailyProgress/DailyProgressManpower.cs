using Construction.Domain.Common;

namespace Construction.Domain.DailyProgress;

public sealed class DailyProgressManpower : AuditableEntity<Guid>
{
    private DailyProgressManpower()
        : base(Guid.Empty)
    {
    }

    internal DailyProgressManpower(
        Guid id,
        Guid reportId,
        Guid? companyId,
        string trade,
        int headcount,
        decimal totalHours)
        : base(id)
    {
        ReportId = reportId;
        CompanyId = companyId;
        Update(companyId, trade, headcount, totalHours);
    }

    public Guid ReportId { get; private set; }

    public Guid? CompanyId { get; private set; }

    public string Trade { get; private set; } = string.Empty;

    public int Headcount { get; private set; }

    public decimal TotalHours { get; private set; }

    internal void Update(
        Guid? companyId,
        string trade,
        int headcount,
        decimal totalHours)
    {
        if (string.IsNullOrWhiteSpace(trade))
        {
            throw new DomainException(
                "Manpower trade is required.");
        }

        if (trade.Trim().Length > 100)
        {
            throw new DomainException(
                "Manpower trade cannot exceed 100 characters.");
        }

        if (headcount <= 0)
        {
            throw new DomainException(
                "Manpower headcount must be greater than zero.");
        }

        if (totalHours < 0)
        {
            throw new DomainException(
                "Manpower total hours cannot be negative.");
        }

        CompanyId = companyId;
        Trade = trade.Trim();
        Headcount = headcount;
        TotalHours = totalHours;
    }
}
