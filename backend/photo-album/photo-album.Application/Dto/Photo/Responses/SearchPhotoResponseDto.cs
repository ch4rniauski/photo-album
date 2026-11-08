namespace photo_album.Application.Dto.Photo.Responses;

public sealed record SearchPhotoResponseDto(
    Guid Id,
    string Name,
    int LikesCount,
    int DislikesCount,
    Guid OwnerId,
    Guid? FolderId,
    string ThumbnailUrl);
