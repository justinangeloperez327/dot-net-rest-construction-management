using Construction.Domain.Common;

namespace Construction.Domain.Locations;

public sealed class ProjectLocation : AuditableEntity<Guid>
{
    private ProjectLocation()
        : base(Guid.Empty)
    {
    }

    private ProjectLocation(
        Guid id,
        Guid projectId,
        string name,
        LocationType type,
        Guid? parentLocationId)
        : base(id)
    {
        ProjectId = projectId;
        Name = name;
        NormalizedName = Normalize(name);
        Type = type;
        ParentLocationId = parentLocationId;
        IsActive = true;
    }

    public Guid ProjectId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string NormalizedName { get; private set; } = string.Empty;

    public LocationType Type { get; private set; }

    public Guid? ParentLocationId { get; private set; }

    public bool IsActive { get; private set; }

    public static ProjectLocation Create(
        Guid projectId,
        string name,
        LocationType type,
        Guid? parentLocationId)
    {
        if (projectId == Guid.Empty)
        {
            throw new DomainException(
                "Project identifier is required.");
        }

        ValidateName(name);

        return new ProjectLocation(
            Guid.CreateVersion7(),
            projectId,
            name.Trim(),
            type,
            parentLocationId);
    }

    public void Update(
        string name,
        LocationType type,
        Guid? parentLocationId)
    {
        if (!IsActive)
        {
            throw new DomainException(
                "Inactive locations cannot be updated.");
        }

        if (parentLocationId == Id)
        {
            throw new DomainException(
                "A project location cannot be its own parent.");
        }

        ValidateName(name);

        Name = name.Trim();
        NormalizedName = Normalize(name);
        Type = type;
        ParentLocationId = parentLocationId;
    }

    public void Deactivate() => IsActive = false;

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException(
                "Location name is required.");
        }

        if (name.Trim().Length > 200)
        {
            throw new DomainException(
                "Location name cannot exceed 200 characters.");
        }
    }

    private static string Normalize(string value) =>
        value.Trim().ToUpperInvariant();
}
