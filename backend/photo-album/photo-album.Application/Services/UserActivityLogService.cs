using photo_album.Application.Contracts.Activity;
using photo_album.Application.Contracts.Repositories;
using photo_album.Domain.Entities;

namespace photo_album.Application.Services;

internal sealed class UserActivityLogService : IUserActivityLogService
{
    private readonly IUserActivityRepository _userActivityRepository;

    public UserActivityLogService(IUserActivityRepository userActivityRepository)
    {
        _userActivityRepository = userActivityRepository;
    }

    public Task LogAsync(
        Guid? userId,
        string userName,
        string action,
        string? entityType = null,
        Guid? entityId = null,
        string? details = null,
        CancellationToken cancellationToken = default)
    {
        var activity = new UserActivityEntity
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            UserName = userName,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            Details = details,
            CreatedAtUtc = DateTime.UtcNow
        };

        return _userActivityRepository.AddAsync(activity, cancellationToken);
    }
}
