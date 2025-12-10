using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialApp.Domain.Entities;

namespace SocialApp.Data.Configurations;

public sealed class LikeConfiguration : BaseEntityConfiguration<Like>
{
    public override void Configure(EntityTypeBuilder<Like> builder)
    {
        builder.ToTable("Likes");

        builder.Property(l => l.UserId).IsRequired();
        builder.Property(l => l.PostId).IsRequired();

        builder.HasIndex(l => new { l.UserId, l.PostId }).IsUnique();

        builder.HasOne(l => l.Post)
            .WithMany(p => p.Likes)
            .HasForeignKey(l => l.PostId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(l => l.User)
               .WithMany(u => u.Likes)
               .HasForeignKey(l => l.UserId)
               .OnDelete(DeleteBehavior.NoAction);
    }
}