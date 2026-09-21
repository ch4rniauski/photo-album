using Microsoft.EntityFrameworkCore;
using photo_album.Application.Contracts.Repositories;
using photo_album.Domain.Entities;

namespace photo_album.Infrastructure.Repositories;

internal sealed class PhotoReactionRepository : IPhotoReactionRepository
{
    private readonly PhotoAlbumContext _context;

    public PhotoReactionRepository(PhotoAlbumContext context)
    {
        _context = context;
    }

    public Task<PhotoReactionEntity?> GetByPhotoIdAndUserIdAsync(
        Guid photoId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return _context.PhotoReactions
            .FirstOrDefaultAsync(
                reaction => reaction.PhotoId == photoId &&
                            reaction.UserId == userId,
                cancellationToken);
    }

    public async Task<bool> AddAsync(
        PhotoReactionEntity reaction,
        CancellationToken cancellationToken = default)
    {
        await _context.PhotoReactions.AddAsync(reaction, cancellationToken);

        return await _context.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> UpdateAsync(
        PhotoReactionEntity reaction,
        CancellationToken cancellationToken = default)
    {
        _context.PhotoReactions.Update(reaction);

        return await _context.SaveChangesAsync(cancellationToken) > 0;
    }
}
