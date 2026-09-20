namespace photo_album.Application.Dto.Photo.Requests;

public sealed record UploadPhotoRequestDto(
    string Name,
    string ContentType,
    string OriginalFileName,
    long Length);
