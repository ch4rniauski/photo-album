using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using photo_album.Domain.Entities;

namespace photo_album.Infrastructure.Configurations;

internal sealed class PhotoEntityConfiguration : IEntityTypeConfiguration<PhotoEntity>
{
    public void Configure(EntityTypeBuilder<PhotoEntity> builder)
    {
        builder.ToTable("Photos");

        builder.HasKey(photo => photo.Id);

        builder.Property(photo => photo.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(photo => photo.OriginalFileName)
            .IsRequired()
            .HasMaxLength(260);

        builder.Property(photo => photo.ThumbnailFileName)
            .IsRequired()
            .HasMaxLength(260);

        builder.Property(photo => photo.LikesCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(photo => photo.DislikesCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(photo => photo.CreatedAtUtc)
            .IsRequired();

        builder.HasOne(photo => photo.Owner)
            .WithMany(user => user.Photos)
            .HasForeignKey(photo => photo.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
