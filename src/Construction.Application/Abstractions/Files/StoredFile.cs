namespace Construction.Application.Abstractions.Files;

public sealed record StoredFile(
    string StorageKey,
    string FileName,
    string ContentType,
    long Length);
