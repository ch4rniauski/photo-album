using Microsoft.EntityFrameworkCore;
using photo_album.Domain.Entities;

namespace photo_album.Infrastructure;

public class PhotoAlbumContext : DbContext
{
    public DbSet<UserEntity> Users { get; set; }
    
    public PhotoAlbumContext(DbContextOptions<PhotoAlbumContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(PhotoAlbumContext).Assembly);
}
