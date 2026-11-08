using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using photo_album.Domain.Entities;

namespace photo_album.Infrastructure.Configurations;

internal sealed class FolderEntityConfiguration : IEntityTypeConfiguration<FolderEntity>
{
    public void Configure(EntityTypeBuilder<FolderEntity> builder)
    {
        builder.ToTable("Folders");

        builder.HasKey(folder => folder.Id);

        builder.Property(folder => folder.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(folder => folder.CreatedAtUtc)
            .IsRequired();

        builder.HasOne(folder => folder.Owner)
            .WithMany(user => user.Folders)
            .HasForeignKey(folder => folder.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(folder => folder.OwnerId);
        builder.HasIndex(folder => folder.CreatedAtUtc);
    }
}
