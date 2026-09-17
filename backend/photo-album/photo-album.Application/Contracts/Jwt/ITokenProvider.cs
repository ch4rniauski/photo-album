using photo_album.Domain.Entities;

namespace photo_album.Application.Contracts.Jwt;

public interface ITokenProvider
{
    string GenerateAccessToken(UserEntity client);
    string GenerateRefreshToken();
}
