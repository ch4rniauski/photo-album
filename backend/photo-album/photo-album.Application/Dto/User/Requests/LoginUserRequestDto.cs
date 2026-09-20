namespace photo_album.Application.Dto.User.Requests;

public sealed record LoginUserRequestDto(
    string Email,
    string Password);
