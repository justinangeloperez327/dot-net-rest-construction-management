using Construction.Domain.Common;

namespace Construction.Domain.Projects;

public sealed class Project : AuditableAggregateRoot<Guid>
{
    private Project()
        : base(Guid.Empty)
    {
    }

    private Project(
        Guid id,
        string number,
        string name,
        string? description,
        DateOnly? startDate,
        DateOnly? plannedEndDate,
        Guid? clientCompanyId,
        Guid? mainContractorCompanyId,
        Guid? consultantCompanyId)
        : base(id)
    {
        Number = number;
        NormalizedNumber = NormalizeNumber(number);
        Name = name;
        Description = description;
        StartDate = startDate;
        PlannedEndDate = plannedEndDate;
        ClientCompanyId = clientCompanyId;
        MainContractorCompanyId = mainContractorCompanyId;
        ConsultantCompanyId = consultantCompanyId;
        Status = ProjectStatus.Planned;
    }

    public string Number { get; private set; } = string.Empty;

    public string NormalizedNumber { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public ProjectStatus Status { get; private set; }

    public DateOnly? StartDate { get; private set; }

    public DateOnly? PlannedEndDate { get; private set; }

    public DateOnly? ActualEndDate { get; private set; }

    public Guid? ClientCompanyId { get; private set; }

    public Guid? MainContractorCompanyId { get; private set; }

    public Guid? ConsultantCompanyId { get; private set; }

    public static Project Create(
        string number,
        string name,
        string? description,
        DateOnly? startDate,
        DateOnly? plannedEndDate,
        Guid? clientCompanyId,
        Guid? mainContractorCompanyId,
        Guid? consultantCompanyId)
    {
        ValidateNumber(number);
        ValidateDetails(name, description, startDate, plannedEndDate);

        return new Project(
            Guid.CreateVersion7(),
            number.Trim(),
            name.Trim(),
            NormalizeDescription(description),
            startDate,
            plannedEndDate,
            clientCompanyId,
            mainContractorCompanyId,
            consultantCompanyId);
    }

    public void Update(
        string name,
        string? description,
        DateOnly? startDate,
        DateOnly? plannedEndDate,
        Guid? clientCompanyId,
        Guid? mainContractorCompanyId,
        Guid? consultantCompanyId)
    {
        if (Status == ProjectStatus.Archived)
        {
            throw new DomainException(
                "Archived projects cannot be updated.");
        }

        ValidateDetails(name, description, startDate, plannedEndDate);

        Name = name.Trim();
        Description = NormalizeDescription(description);
        StartDate = startDate;
        PlannedEndDate = plannedEndDate;
        ClientCompanyId = clientCompanyId;
        MainContractorCompanyId = mainContractorCompanyId;
        ConsultantCompanyId = consultantCompanyId;
    }

    public void Start()
    {
        if (Status is not ProjectStatus.Planned
            and not ProjectStatus.OnHold)
        {
            throw new DomainException(
                "Only planned or on-hold projects can be started.");
        }

        Status = ProjectStatus.Active;
    }

    public void PutOnHold()
    {
        if (Status != ProjectStatus.Active)
        {
            throw new DomainException(
                "Only active projects can be placed on hold.");
        }

        Status = ProjectStatus.OnHold;
    }

    public void Complete(DateOnly completionDate)
    {
        if (Status is ProjectStatus.Completed
            or ProjectStatus.Archived)
        {
            throw new DomainException(
                "The project cannot be completed from its current status.");
        }

        if (StartDate is DateOnly startDate
            && completionDate < startDate)
        {
            throw new DomainException(
                "Completion date cannot be earlier than the project start date.");
        }

        Status = ProjectStatus.Completed;
        ActualEndDate = completionDate;
    }

    public void Archive()
    {
        if (Status == ProjectStatus.Archived)
        {
            return;
        }

        Status = ProjectStatus.Archived;
    }

    private static void ValidateNumber(string number)
    {
        if (string.IsNullOrWhiteSpace(number))
        {
            throw new DomainException(
                "Project number is required.");
        }

        if (number.Trim().Length > 50)
        {
            throw new DomainException(
                "Project number cannot exceed 50 characters.");
        }
    }

    private static void ValidateDetails(
        string name,
        string? description,
        DateOnly? startDate,
        DateOnly? plannedEndDate)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException(
                "Project name is required.");
        }

        if (name.Trim().Length > 200)
        {
            throw new DomainException(
                "Project name cannot exceed 200 characters.");
        }

        if (description?.Trim().Length > 2000)
        {
            throw new DomainException(
                "Project description cannot exceed 2000 characters.");
        }

        if (startDate is DateOnly start
            && plannedEndDate is DateOnly end
            && end < start)
        {
            throw new DomainException(
                "Planned end date cannot be earlier than the start date.");
        }
    }

    private static string NormalizeNumber(string value) =>
        value.Trim().ToUpperInvariant();

    private static string? NormalizeDescription(string? value) =>
        string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
}
