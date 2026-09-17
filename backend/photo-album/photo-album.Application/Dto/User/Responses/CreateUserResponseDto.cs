namespace photo_album.Application.Dto.User.Responses;

public sealed record CreateUserResponseDto(
    Guid Id,
    string UserName,
    string Email,
    string DisplayName,
    string Role);
