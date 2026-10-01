namespace Construction.Infrastructure.Files;

public sealed class FileStorageOptions
{
    public const string SectionName = "FileStorage";

    public string RootPath { get; init; } = string.Empty;

    public long MaximumFileSizeBytes { get; init; } = 104_857_600;
}
