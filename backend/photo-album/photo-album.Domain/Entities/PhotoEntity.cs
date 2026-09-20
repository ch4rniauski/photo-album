namespace photo_album.Domain.Entities;

public sealed class PhotoEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string ThumbnailFileName { get; set; } = string.Empty;
    public int LikesCount { get; set; }
    public int DislikesCount { get; set; }
    public Guid OwnerId { get; set; }
    public UserEntity Owner { get; set; } = null!;
    public DateTime CreatedAtUtc { get; set; }
}
