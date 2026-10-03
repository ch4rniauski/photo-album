using Microsoft.EntityFrameworkCore;
using photo_album.Application.Contracts.Repositories;
using photo_album.Domain.Entities;

namespace photo_album.Infrastructure.Repositories;

internal sealed class UserRepository : IUserRepository
{
    private readonly PhotoAlbumContext _context;

    public UserRepository(PhotoAlbumContext context)
    {
        _context = context;
    }

    public async Task<bool> AddAsync(
        UserEntity user,
        CancellationToken cancellationToken = default)
    {
        await _context.Users.AddAsync(user, cancellationToken);

        return await _context.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> UpdateAsync(
        UserEntity user,
        CancellationToken cancellationToken = default)
    {
        _context.Users.Update(user);

        return await _context.SaveChangesAsync(cancellationToken) > 0;
    }

    public Task<UserEntity?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _context.Users
            .FirstOrDefaultAsync(user => user.Id == id, cancellationToken);
    }

    public Task<UserEntity?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        return _context.Users
            .FirstOrDefaultAsync(user => user.Email == email, cancellationToken);
    }

    public Task<bool> ExistsByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        return _context.Users
            .AnyAsync(user => user.Email == email, cancellationToken);
    }

    public Task<bool> ExistsByUserNameAsync(
        string userName,
        CancellationToken cancellationToken = default)
    {
        return _context.Users
            .AnyAsync(user => user.UserName == userName, cancellationToken);
    }

    public async Task<(IReadOnlyList<UserEntity> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Users.AsNoTracking();

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(user => user.UserName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
