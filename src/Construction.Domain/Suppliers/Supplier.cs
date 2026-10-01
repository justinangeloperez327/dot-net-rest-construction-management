using Construction.Domain.Common;

namespace Construction.Domain.Suppliers;

public sealed class Supplier : AuditableAggregateRoot<Guid>
{
    private Supplier()
        : base(Guid.Empty)
    {
    }

    private Supplier(
        Guid id,
        Guid companyId,
        string code,
        string? taxRegistrationNumber,
        string? contactName,
        string? contactEmail,
        string? contactPhone,
        string? paymentTerms)
        : base(id)
    {
        CompanyId = companyId;
        Code = code;
        NormalizedCode = Normalize(code);
        Status = SupplierStatus.Active;
        UpdateDetails(
            taxRegistrationNumber,
            contactName,
            contactEmail,
            contactPhone,
            paymentTerms);
    }

    public Guid CompanyId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string NormalizedCode { get; private set; } = string.Empty;
    public SupplierStatus Status { get; private set; }
    public string? TaxRegistrationNumber { get; private set; }
    public string? ContactName { get; private set; }
    public string? ContactEmail { get; private set; }
    public string? ContactPhone { get; private set; }
    public string? PaymentTerms { get; private set; }

    public static Supplier Create(
        Guid companyId,
        string code,
        string? taxRegistrationNumber,
        string? contactName,
        string? contactEmail,
        string? contactPhone,
        string? paymentTerms)
    {
        if (companyId == Guid.Empty)
        {
            throw new DomainException(
                "Supplier company identifier is required.");
        }

        ValidateCode(code);

        return new Supplier(
            Guid.CreateVersion7(),
            companyId,
            code.Trim(),
            taxRegistrationNumber,
            contactName,
            contactEmail,
            contactPhone,
            paymentTerms);
    }

    public void Update(
        string? taxRegistrationNumber,
        string? contactName,
        string? contactEmail,
        string? contactPhone,
        string? paymentTerms)
    {
        if (Status == SupplierStatus.Inactive)
        {
            throw new DomainException(
                "Inactive suppliers cannot be updated.");
        }

        UpdateDetails(
            taxRegistrationNumber,
            contactName,
            contactEmail,
            contactPhone,
            paymentTerms);
    }

    public void Activate() => Status = SupplierStatus.Active;

    public void Deactivate() => Status = SupplierStatus.Inactive;

    private void UpdateDetails(
        string? taxRegistrationNumber,
        string? contactName,
        string? contactEmail,
        string? contactPhone,
        string? paymentTerms)
    {
        TaxRegistrationNumber = NormalizeOptional(taxRegistrationNumber, 100);
        ContactName = NormalizeOptional(contactName, 200);
        ContactEmail = NormalizeOptional(contactEmail, 320);
        ContactPhone = NormalizeOptional(contactPhone, 50);
        PaymentTerms = NormalizeOptional(paymentTerms, 500);
    }

    private static void ValidateCode(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException("Supplier code is required.");
        }

        if (value.Trim().Length > 50)
        {
            throw new DomainException(
                "Supplier code cannot exceed 50 characters.");
        }
    }

    private static string Normalize(string value) =>
        value.Trim().ToUpperInvariant();

    private static string? NormalizeOptional(
        string? value,
        int maximumLength)
    {
        if (value?.Trim().Length > maximumLength)
        {
            throw new DomainException(
                $"Value cannot exceed {maximumLength} characters.");
        }

        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
