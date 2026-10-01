using Construction.Application.Abstractions.Files;

namespace Construction.Infrastructure.Files;

public sealed class LocalFileStorage : IFileStorage
{
    private readonly string _rootPath;
    private readonly long _maximumFileSizeBytes;

    public LocalFileStorage(FileStorageOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentException.ThrowIfNullOrWhiteSpace(options.RootPath);

        if (options.MaximumFileSizeBytes <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(options),
                "Maximum file size must be greater than zero.");
        }

        _rootPath = Path.GetFullPath(options.RootPath);
        _maximumFileSizeBytes = options.MaximumFileSizeBytes;

        Directory.CreateDirectory(_rootPath);
    }

    public async Task<StoredFile> SaveAsync(
        FileUpload file,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(file);
        ArgumentNullException.ThrowIfNull(file.Content);

        string safeFileName = Path.GetFileName(file.FileName);

        if (string.IsNullOrWhiteSpace(safeFileName)
            || safeFileName.Length > 255)
        {
            throw new InvalidDataException(
                "File name is required and cannot exceed 255 characters.");
        }

        if (string.IsNullOrWhiteSpace(file.ContentType)
            || file.ContentType.Length > 200)
        {
            throw new InvalidDataException(
                "File content type is required and cannot exceed 200 characters.");
        }

        if (file.Length <= 0)
        {
            throw new InvalidDataException("File cannot be empty.");
        }

        if (file.Length > _maximumFileSizeBytes)
        {
            throw new InvalidDataException(
                $"File exceeds the maximum size of {_maximumFileSizeBytes} bytes.");
        }

        string storageKey = Guid.CreateVersion7().ToString("N");
        string path = GetStoragePath(storageKey);

        try
        {
            await using var output = new FileStream(
                path,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None,
                bufferSize: 81_920,
                FileOptions.Asynchronous | FileOptions.SequentialScan);

            await file.Content.CopyToAsync(output, cancellationToken);
            await output.FlushAsync(cancellationToken);

            if (output.Length != file.Length)
            {
                throw new InvalidDataException(
                    "Uploaded file length does not match the declared file length.");
            }
        }
        catch
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            throw;
        }

        return new StoredFile(
            storageKey,
            safeFileName,
            file.ContentType.Trim(),
            file.Length);
    }

    public Task<Stream?> OpenReadAsync(
        string storageKey,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string path = GetStoragePath(storageKey);

        if (!File.Exists(path))
        {
            return Task.FromResult<Stream?>(null);
        }

        Stream stream = new FileStream(
            path,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 81_920,
            FileOptions.Asynchronous | FileOptions.SequentialScan);

        return Task.FromResult<Stream?>(stream);
    }

    public Task DeleteAsync(
        string storageKey,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string path = GetStoragePath(storageKey);

        if (File.Exists(path))
        {
            File.Delete(path);
        }

        return Task.CompletedTask;
    }

    private string GetStoragePath(string storageKey)
    {
        if (string.IsNullOrWhiteSpace(storageKey)
            || storageKey.Length > 100
            || storageKey.Any(character =>
                !char.IsAsciiHexDigit(character)))
        {
            throw new InvalidDataException("Storage key is invalid.");
        }

        string path = Path.GetFullPath(
            Path.Combine(_rootPath, storageKey));

        string rootPrefix = _rootPath.EndsWith(
            Path.DirectorySeparatorChar.ToString(),
            StringComparison.Ordinal)
                ? _rootPath
                : _rootPath + Path.DirectorySeparatorChar;

        if (!path.StartsWith(
            rootPrefix,
            StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidDataException(
                "Storage path escapes the configured root.");
        }

        return path;
    }
}
