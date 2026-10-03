namespace photo_album.Application.Dto.Admin.Responses;

public sealed record UserActivityResponseDto(
    Guid Id,
    Guid? UserId,
    string UserName,
    string Action,
    string? EntityType,
    Guid? EntityId,
    string? Details,
    DateTime CreatedAtUtc);
