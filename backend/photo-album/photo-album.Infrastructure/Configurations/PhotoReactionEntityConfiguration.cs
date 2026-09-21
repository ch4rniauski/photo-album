using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using photo_album.Domain.Entities;

namespace photo_album.Infrastructure.Configurations;

internal sealed class PhotoReactionEntityConfiguration : IEntityTypeConfiguration<PhotoReactionEntity>
{
    public void Configure(EntityTypeBuilder<PhotoReactionEntity> builder)
    {
        builder.ToTable("PhotoReactions");

        builder.HasKey(reaction => new { reaction.PhotoId, reaction.UserId });

        builder.Property(reaction => reaction.IsLiked)
            .IsRequired();

        builder.HasOne(reaction => reaction.Photo)
            .WithMany(photo => photo.Reactions)
            .HasForeignKey(reaction => reaction.PhotoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(reaction => reaction.User)
            .WithMany(user => user.PhotoReactions)
            .HasForeignKey(reaction => reaction.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
