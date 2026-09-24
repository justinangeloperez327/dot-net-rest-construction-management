namespace Construction.Application.Abstractions.Authentication;

public interface IUserDirectory
{
    Task<bool> ExistsAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}
