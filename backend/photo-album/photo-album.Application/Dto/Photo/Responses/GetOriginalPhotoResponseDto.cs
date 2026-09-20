namespace photo_album.Application.Dto.Photo.Responses;

public sealed record GetOriginalPhotoResponseDto(
    Stream Content,
    string ContentType,
    string FileName);
