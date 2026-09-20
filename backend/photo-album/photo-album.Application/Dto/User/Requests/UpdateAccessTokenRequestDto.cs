namespace photo_album.Application.Dto.User.Requests;

public sealed record UpdateAccessTokenRequestDto(
    string UserId,
    string RefreshToken);
