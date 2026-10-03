using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using photo_album.Domain.Entities;

namespace photo_album.Infrastructure.Configurations;

internal sealed class UserActivityEntityConfiguration : IEntityTypeConfiguration<UserActivityEntity>
{
    public void Configure(EntityTypeBuilder<UserActivityEntity> builder)
    {
        builder.ToTable("UserActivities");

        builder.HasKey(activity => activity.Id);

        builder.Property(activity => activity.UserName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(activity => activity.Action)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(activity => activity.EntityType)
            .IsRequired(false)
            .HasMaxLength(50);

        builder.Property(activity => activity.Details)
            .IsRequired(false)
            .HasMaxLength(500);

        builder.Property(activity => activity.CreatedAtUtc)
            .IsRequired();

        builder.HasIndex(activity => activity.CreatedAtUtc);
        builder.HasIndex(activity => activity.UserId);
        builder.HasIndex(activity => activity.Action);
    }
}
