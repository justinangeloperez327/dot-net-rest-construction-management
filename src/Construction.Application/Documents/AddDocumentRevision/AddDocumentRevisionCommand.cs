using Construction.Application.Abstractions.Files;
using Construction.Application.Common.Messaging;

namespace Construction.Application.Documents.AddDocumentRevision;

public sealed record AddDocumentRevisionCommand(
    Guid ProjectId,
    Guid DocumentId,
    string RevisionCode,
    string? Notes,
    FileUpload File) : ICommand<DocumentRevisionResponse>;
