using Microsoft.EntityFrameworkCore;
using photo_album.Application.Contracts.Repositories;
using photo_album.Domain.Entities;

namespace photo_album.Infrastructure.Repositories;

internal sealed class FolderRepository : IFolderRepository
{
    private readonly PhotoAlbumContext _context;

    public FolderRepository(PhotoAlbumContext context)
    {
        _context = context;
    }

    public async Task<bool> AddAsync(FolderEntity folder, CancellationToken cancellationToken = default)
    {
        await _context.Folders.AddAsync(folder, cancellationToken);

        return await _context.SaveChangesAsync(cancellationToken) > 0;
    }

    public Task<FolderEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _context.Folders
            .FirstOrDefaultAsync(folder => folder.Id == id, cancellationToken);
    }

    public Task<FolderEntity?> GetByIdWithOwnerAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _context.Folders
            .AsNoTracking()
            .Include(folder => folder.Owner)
            .FirstOrDefaultAsync(folder => folder.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<FolderEntity>> GetWithPaginationAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        return await _context.Folders
            .AsNoTracking()
            .Include(folder => folder.Owner)
            .OrderByDescending(folder => folder.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<FolderEntity>> GetByOwnerIdAsync(
        Guid ownerId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Folders
            .AsNoTracking()
            .Include(folder => folder.Owner)
            .Where(folder => folder.OwnerId == ownerId)
            .OrderByDescending(folder => folder.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public Task<int> GetPhotosCountAsync(Guid folderId, CancellationToken cancellationToken = default)
    {
        return _context.Photos
            .AsNoTracking()
            .CountAsync(photo => photo.FolderId == folderId, cancellationToken);
    }

    public async Task<bool> DeleteAsync(
        FolderEntity folder,
        CancellationToken cancellationToken = default)
    {
        var deletedCount = await _context.Folders
            .Where(entity => entity.Id == folder.Id)
            .ExecuteDeleteAsync(cancellationToken);

        return deletedCount > 0;
    }
}
