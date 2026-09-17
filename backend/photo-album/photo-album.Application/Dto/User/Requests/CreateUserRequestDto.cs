namespace photo_album.Application.Dto.User.Requests;

public sealed record CreateUserRequestDto(
    string UserName,
    string Email,
    string Password,
    string DisplayName);
