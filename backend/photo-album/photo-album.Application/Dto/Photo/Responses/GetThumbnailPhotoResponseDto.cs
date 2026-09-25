namespace photo_album.Application.Dto.Photo.Responses;

public sealed record GetThumbnailPhotoResponseDto(
    Stream Content,
    string ContentType,
    string FileName);
