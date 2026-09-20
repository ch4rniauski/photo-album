namespace photo_album.Application.Dto.Photo.Responses;

public sealed record UploadPhotoResponseDto(
    Guid Id,
    string Name,
    string OriginalFileName,
    string ThumbnailFileName,
    int LikesCount,
    int DislikesCount,
    Guid OwnerId);
