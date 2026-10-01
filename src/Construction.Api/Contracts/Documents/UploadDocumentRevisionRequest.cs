namespace Construction.Api.Contracts.Documents;

public sealed class UploadDocumentRevisionRequest
{
    public required string RevisionCode { get; init; }

    public string? Notes { get; init; }

    public required IFormFile File { get; init; }
}
