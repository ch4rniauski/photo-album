namespace frontend.Models;

public sealed record FolderDto(
    Guid Id,
    string Name,
    Guid OwnerId,
    string OwnerUserName,
    int PhotosCount,
    DateTime CreatedAtUtc);
