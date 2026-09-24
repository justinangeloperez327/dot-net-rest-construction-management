namespace Construction.Application.Abstractions.Files;

public sealed record FileUpload(
    Stream Content,
    string FileName,
    string ContentType,
    long Length);
