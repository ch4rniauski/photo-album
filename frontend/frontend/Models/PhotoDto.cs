namespace frontend.Models;

public sealed record PhotoDto(
    Guid Id,
    string Name,
    int LikesCount,
    int DislikesCount,
    Guid OwnerId,
    string ThumbnailUrl);
