using photo_album.Domain.Entities;

namespace photo_album.Application.Contracts.Repositories;

public interface IUserActivityRepository
{
    Task<bool> AddAsync(UserActivityEntity activity, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<UserActivityEntity> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        Guid? userId,
        string? action,
        CancellationToken cancellationToken = default);
}
