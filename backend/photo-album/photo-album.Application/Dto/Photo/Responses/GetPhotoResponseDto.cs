namespace photo_album.Application.Dto.Photo.Responses;

public sealed record GetPhotoResponseDto(
    Guid Id,
    string Name,
    int LikesCount,
    int DislikesCount,
    Guid OwnerId,
    string ThumbnailUrl);
