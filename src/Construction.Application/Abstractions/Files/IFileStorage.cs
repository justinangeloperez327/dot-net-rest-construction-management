namespace Construction.Application.Abstractions.Files;

public interface IFileStorage
{
    Task<StoredFile> SaveAsync(
        FileUpload file,
        CancellationToken cancellationToken = default);

    Task<Stream?> OpenReadAsync(
        string storageKey,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        string storageKey,
        CancellationToken cancellationToken = default);
}
