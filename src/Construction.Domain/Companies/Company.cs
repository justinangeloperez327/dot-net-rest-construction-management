using Construction.Domain.Common;

namespace Construction.Domain.Companies;

public sealed class Company : AuditableAggregateRoot<Guid>
{
    private Company()
        : base(Guid.Empty)
    {
    }

    private Company(
        Guid id,
        string name,
        CompanyType type)
        : base(id)
    {
        Name = name;
        NormalizedName = Normalize(name);
        Type = type;
        Status = CompanyStatus.Active;
    }

    public string Name { get; private set; } = string.Empty;

    public string NormalizedName { get; private set; } = string.Empty;

    public CompanyType Type { get; private set; }

    public CompanyStatus Status { get; private set; }

    public static Company Create(
        string name,
        CompanyType type)
    {
        ValidateName(name);

        return new Company(
            Guid.CreateVersion7(),
            name.Trim(),
            type);
    }

    public void Update(
        string name,
        CompanyType type)
    {
        ValidateName(name);

        Name = name.Trim();
        NormalizedName = Normalize(name);
        Type = type;
    }

    public void Activate() => Status = CompanyStatus.Active;

    public void Deactivate() => Status = CompanyStatus.Inactive;

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException(
                "Company name is required.");
        }

        if (name.Trim().Length > 200)
        {
            throw new DomainException(
                "Company name cannot exceed 200 characters.");
        }
    }

    private static string Normalize(string value) =>
        value.Trim().ToUpperInvariant();
}
