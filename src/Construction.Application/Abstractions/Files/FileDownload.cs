namespace Construction.Application.Abstractions.Files;

public sealed record FileDownload(
    Stream Content,
    string FileName,
    string ContentType,
    long Length);
