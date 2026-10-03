namespace photo_album.Application.Dto.Admin.Responses;

public sealed record AdminUserResponseDto(
    Guid Id,
    string UserName,
    string Email,
    string Role);
