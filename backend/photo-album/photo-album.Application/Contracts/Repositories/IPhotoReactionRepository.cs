using photo_album.Domain.Entities;

namespace photo_album.Application.Contracts.Repositories;

public interface IPhotoReactionRepository
{
    Task<PhotoReactionEntity?> GetByPhotoIdAndUserIdAsync(
        Guid photoId,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<bool> AddAsync(PhotoReactionEntity reaction, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(PhotoReactionEntity reaction, CancellationToken cancellationToken = default);
}
