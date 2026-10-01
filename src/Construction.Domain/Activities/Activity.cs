using Construction.Domain.Common;

namespace Construction.Domain.Activities;

public sealed class Activity : AuditableAggregateRoot<Guid>
{
    private Activity()
        : base(Guid.Empty)
    {
    }

    private Activity(
        Guid id,
        Guid projectId,
        string code,
        string name,
        string? description,
        Guid? workPackageId,
        Guid? locationId,
        ActivityPriority priority,
        DateOnly? plannedStartDate,
        DateOnly? plannedEndDate)
        : base(id)
    {
        ProjectId = projectId;
        Code = code;
        NormalizedCode = Normalize(code);
        Name = name;
        Description = NormalizeDescription(description);
        WorkPackageId = workPackageId;
        LocationId = locationId;
        Priority = priority;
        PlannedStartDate = plannedStartDate;
        PlannedEndDate = plannedEndDate;
        Status = ActivityStatus.NotStarted;
    }

    public Guid ProjectId { get; private set; }

    public string Code { get; private set; } = string.Empty;

    public string NormalizedCode { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public Guid? WorkPackageId { get; private set; }

    public Guid? LocationId { get; private set; }

    public ActivityPriority Priority { get; private set; }

    public ActivityStatus Status { get; private set; }

    public decimal ProgressPercentage { get; private set; }

    public DateOnly? PlannedStartDate { get; private set; }

    public DateOnly? PlannedEndDate { get; private set; }

    public DateOnly? ActualStartDate { get; private set; }

    public DateOnly? ActualEndDate { get; private set; }

    public static Activity Create(
        Guid projectId,
        string code,
        string name,
        string? description,
        Guid? workPackageId,
        Guid? locationId,
        ActivityPriority priority,
        DateOnly? plannedStartDate,
        DateOnly? plannedEndDate)
    {
        if (projectId == Guid.Empty)
        {
            throw new DomainException("Project identifier is required.");
        }

        ValidateCode(code);
        ValidateDetails(name, description, plannedStartDate, plannedEndDate);

        return new Activity(
            Guid.CreateVersion7(),
            projectId,
            code.Trim(),
            name.Trim(),
            description,
            workPackageId,
            locationId,
            priority,
            plannedStartDate,
            plannedEndDate);
    }

    public void Update(
        string name,
        string? description,
        Guid? workPackageId,
        Guid? locationId,
        ActivityPriority priority,
        DateOnly? plannedStartDate,
        DateOnly? plannedEndDate)
    {
        if (Status is ActivityStatus.Completed
            or ActivityStatus.Cancelled)
        {
            throw new DomainException(
                "Completed or cancelled activities cannot be updated.");
        }

        ValidateDetails(name, description, plannedStartDate, plannedEndDate);

        Name = name.Trim();
        Description = NormalizeDescription(description);
        WorkPackageId = workPackageId;
        LocationId = locationId;
        Priority = priority;
        PlannedStartDate = plannedStartDate;
        PlannedEndDate = plannedEndDate;
    }

    public void UpdateProgress(decimal percentage)
    {
        if (percentage is < 0 or > 100)
        {
            throw new DomainException(
                "Activity progress must be between 0 and 100.");
        }

        if (Status is ActivityStatus.Completed
            or ActivityStatus.Cancelled)
        {
            throw new DomainException(
                "Progress cannot be changed for completed or cancelled activities.");
        }

        ProgressPercentage = percentage;
    }

    public void Start(DateOnly startedOn)
    {
        if (Status is not ActivityStatus.NotStarted
            and not ActivityStatus.OnHold)
        {
            throw new DomainException(
                "Only not-started or on-hold activities can be started.");
        }

        Status = ActivityStatus.InProgress;
        ActualStartDate ??= startedOn;
    }

    public void PutOnHold()
    {
        if (Status != ActivityStatus.InProgress)
        {
            throw new DomainException(
                "Only in-progress activities can be placed on hold.");
        }

        Status = ActivityStatus.OnHold;
    }

    public void Complete(DateOnly completedOn)
    {
        if (Status is ActivityStatus.Completed
            or ActivityStatus.Cancelled)
        {
            throw new DomainException(
                "The activity cannot be completed from its current status.");
        }

        if (ActualStartDate is DateOnly startedOn
            && completedOn < startedOn)
        {
            throw new DomainException(
                "Completion date cannot be earlier than the actual start date.");
        }

        Status = ActivityStatus.Completed;
        ProgressPercentage = 100;
        ActualStartDate ??= completedOn;
        ActualEndDate = completedOn;
    }

    public void Cancel()
    {
        if (Status == ActivityStatus.Completed)
        {
            throw new DomainException(
                "Completed activities cannot be cancelled.");
        }

        Status = ActivityStatus.Cancelled;
    }

    private static void ValidateCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new DomainException("Activity code is required.");
        }

        if (code.Trim().Length > 50)
        {
            throw new DomainException(
                "Activity code cannot exceed 50 characters.");
        }
    }

    private static void ValidateDetails(
        string name,
        string? description,
        DateOnly? plannedStartDate,
        DateOnly? plannedEndDate)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Activity name is required.");
        }

        if (name.Trim().Length > 200)
        {
            throw new DomainException(
                "Activity name cannot exceed 200 characters.");
        }

        if (description?.Trim().Length > 4000)
        {
            throw new DomainException(
                "Activity description cannot exceed 4000 characters.");
        }

        if (plannedStartDate is DateOnly start
            && plannedEndDate is DateOnly end
            && end < start)
        {
            throw new DomainException(
                "Planned end date cannot be earlier than the planned start date.");
        }
    }

    private static string Normalize(string value) =>
        value.Trim().ToUpperInvariant();

    private static string? NormalizeDescription(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
