namespace photo_album.Application.Contracts.Activity;

public interface IUserActivityLogService
{
    Task LogAsync(
        Guid? userId,
        string userName,
        string action,
        string? entityType = null,
        Guid? entityId = null,
        string? details = null,
        CancellationToken cancellationToken = default);
}
