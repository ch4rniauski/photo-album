namespace frontend.Models;

public sealed record PhotoMutationResponseDto(
    Guid Id,
    string Name,
    string OriginalFileName,
    string ThumbnailFileName,
    int LikesCount,
    int DislikesCount,
    Guid OwnerId);
