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

    public Task<PhotoEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _context.Photos
            .FirstOrDefaultAsync(photo => photo.Id == id, cancellationToken);
    }
}
