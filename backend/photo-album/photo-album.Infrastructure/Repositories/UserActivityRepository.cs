using Microsoft.EntityFrameworkCore;
using photo_album.Application.Contracts.Repositories;
using photo_album.Domain.Entities;

namespace photo_album.Infrastructure.Repositories;

internal sealed class UserActivityRepository : IUserActivityRepository
{
    private readonly PhotoAlbumContext _context;

    public UserActivityRepository(PhotoAlbumContext context)
    {
        _context = context;
    }

    public async Task<bool> AddAsync(
        UserActivityEntity activity,
        CancellationToken cancellationToken = default)
    {
        await _context.UserActivities.AddAsync(activity, cancellationToken);

        return await _context.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<(IReadOnlyList<UserActivityEntity> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        Guid? userId,
        string? action,
        CancellationToken cancellationToken = default)
    {
        var query = _context.UserActivities.AsNoTracking();

        if (userId is not null)
        {
            query = query.Where(activity => activity.UserId == userId);
        }

        if (!string.IsNullOrWhiteSpace(action))
        {
            query = query.Where(activity => activity.Action == action);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(activity => activity.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
