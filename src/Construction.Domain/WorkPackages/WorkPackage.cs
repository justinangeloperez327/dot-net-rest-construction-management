using Construction.Domain.Common;

namespace Construction.Domain.WorkPackages;

public sealed class WorkPackage : AuditableAggregateRoot<Guid>
{
    private WorkPackage()
        : base(Guid.Empty)
    {
    }

    private WorkPackage(
        Guid id,
        Guid projectId,
        string code,
        string name,
        string? description,
        Guid? parentWorkPackageId)
        : base(id)
    {
        ProjectId = projectId;
        Code = code;
        NormalizedCode = Normalize(code);
        Name = name;
        Description = NormalizeDescription(description);
        ParentWorkPackageId = parentWorkPackageId;
        Status = WorkPackageStatus.Planned;
    }

    public Guid ProjectId { get; private set; }

    public string Code { get; private set; } = string.Empty;

    public string NormalizedCode { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public Guid? ParentWorkPackageId { get; private set; }

    public WorkPackageStatus Status { get; private set; }

    public static WorkPackage Create(
        Guid projectId,
        string code,
        string name,
        string? description,
        Guid? parentWorkPackageId)
    {
        if (projectId == Guid.Empty)
        {
            throw new DomainException("Project identifier is required.");
        }

        ValidateCode(code);
        ValidateName(name);
        ValidateDescription(description);

        return new WorkPackage(
            Guid.CreateVersion7(),
            projectId,
            code.Trim(),
            name.Trim(),
            description,
            parentWorkPackageId);
    }

    public void Update(
        string name,
        string? description,
        Guid? parentWorkPackageId)
    {
        if (Status == WorkPackageStatus.Archived)
        {
            throw new DomainException(
                "Archived work packages cannot be updated.");
        }

        if (parentWorkPackageId == Id)
        {
            throw new DomainException(
                "A work package cannot be its own parent.");
        }

        ValidateName(name);
        ValidateDescription(description);

        Name = name.Trim();
        Description = NormalizeDescription(description);
        ParentWorkPackageId = parentWorkPackageId;
    }

    public void Activate()
    {
        if (Status is not WorkPackageStatus.Planned)
        {
            throw new DomainException(
                "Only planned work packages can be activated.");
        }

        Status = WorkPackageStatus.Active;
    }

    public void Complete()
    {
        if (Status is WorkPackageStatus.Completed
            or WorkPackageStatus.Archived)
        {
            throw new DomainException(
                "The work package cannot be completed from its current status.");
        }

        Status = WorkPackageStatus.Completed;
    }

    public void Archive()
    {
        Status = WorkPackageStatus.Archived;
    }

    private static void ValidateCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new DomainException("Work package code is required.");
        }

        if (code.Trim().Length > 50)
        {
            throw new DomainException(
                "Work package code cannot exceed 50 characters.");
        }
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Work package name is required.");
        }

        if (name.Trim().Length > 200)
        {
            throw new DomainException(
                "Work package name cannot exceed 200 characters.");
        }
    }

    private static void ValidateDescription(string? description)
    {
        if (description?.Trim().Length > 2000)
        {
            throw new DomainException(
                "Work package description cannot exceed 2000 characters.");
        }
    }

    private static string Normalize(string value) =>
        value.Trim().ToUpperInvariant();

    private static string? NormalizeDescription(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
