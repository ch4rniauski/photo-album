using photo_album.Domain.Entities;

namespace photo_album.Application.Contracts.Repositories;

public interface IPhotoRepository
{
    Task<bool> AddAsync(PhotoEntity photo, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(PhotoEntity photo, CancellationToken cancellationToken = default);

    Task<PhotoEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
