using photo_album.Domain.Entities;

namespace photo_album.Application.Contracts.Repositories;

public interface IFolderRepository
{
    Task<bool> AddAsync(FolderEntity folder, CancellationToken cancellationToken = default);

    Task<FolderEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<FolderEntity?> GetByIdWithOwnerAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<FolderEntity>> GetWithPaginationAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<FolderEntity>> GetByOwnerIdAsync(
        Guid ownerId,
        CancellationToken cancellationToken = default);

    Task<int> GetPhotosCountAsync(Guid folderId, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(FolderEntity folder, CancellationToken cancellationToken = default);
}
