namespace photo_album.Application.Dto.Folder.Responses;

public sealed record GetFolderResponseDto(
    Guid Id,
    string Name,
    Guid OwnerId,
    string OwnerUserName,
    int PhotosCount,
    DateTime CreatedAtUtc);
