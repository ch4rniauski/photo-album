namespace photo_album.Application.Dto.User.Responses;

public sealed record LoginUserResponseDto(
    string AccessToken,
    string RefreshToken,
    string UserId);
