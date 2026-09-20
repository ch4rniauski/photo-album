using photo_album.Domain.Entities;

namespace photo_album.Application.Contracts.Repositories;

public interface IUserRepository
{
    Task<bool> AddAsync(UserEntity user, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(UserEntity user, CancellationToken cancellationToken = default);

    Task<UserEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<UserEntity?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<bool> ExistsByUserNameAsync(string userName, CancellationToken cancellationToken = default);
}
