namespace photo_album.Domain.Entities;

public sealed class FolderEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid OwnerId { get; set; }
    public UserEntity Owner { get; set; } = null!;
    public DateTime CreatedAtUtc { get; set; }
    public ICollection<PhotoEntity> Photos { get; set; } = [];
}
