using Microsoft.EntityFrameworkCore;
using photo_album.Application.Contracts.Repositories;
using photo_album.Domain.Entities;

namespace photo_album.Infrastructure.Repositories;

internal sealed class PhotoRepository : IPhotoRepository
{
    private readonly PhotoAlbumContext _context;

    public PhotoRepository(PhotoAlbumContext context)
    {
        _context = context;
    }

    public async Task<bool> AddAsync(PhotoEntity photo, CancellationToken cancellationToken = default)
    {
        await _context.Photos.AddAsync(photo, cancellationToken);

        return await _context.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> UpdateAsync(PhotoEntity photo, CancellationToken cancellationToken = default)
    {
        _context.Photos.Update(photo);

        return await _context.SaveChangesAsync(cancellationToken) > 0;
    }

    public Task<PhotoEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _context.Photos
            .FirstOrDefaultAsync(photo => photo.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<PhotoEntity>> SearchByNameAsync(
        string search,
        CancellationToken cancellationToken = default)
    {
        var pattern = $"%{EscapeLikePattern(search)}%";

        return await _context.Photos
            .AsNoTracking()
            .Where(photo => EF.Functions.ILike(photo.Name, pattern, "\\"))
            .OrderByDescending(photo => photo.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<PhotoEntity>> GetWithPaginationAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        return await _context.Photos
            .AsNoTracking()
            .OrderByDescending(photo => photo.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    private static string EscapeLikePattern(string value)
    {
        return value
            .Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("%", "\\%", StringComparison.Ordinal)
            .Replace("_", "\\_", StringComparison.Ordinal);
    }
}
