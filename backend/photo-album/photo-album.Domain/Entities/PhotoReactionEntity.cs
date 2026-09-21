namespace photo_album.Domain.Entities;

public sealed class PhotoReactionEntity
{
    public Guid PhotoId { get; set; }
    public PhotoEntity Photo { get; set; } = null!;
    public Guid UserId { get; set; }
    public UserEntity User { get; set; } = null!;
    public bool IsLiked { get; set; }
}
