using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialApp.Domain.Entities;

namespace SocialApp.Data.Configurations;

public sealed class PostConfiguration : BaseEntityConfiguration<Post>
{
    public override void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.ToTable("Posts");

        builder.Property(p => p.Body)
            .IsRequired()
            .HasMaxLength(312);

        builder.Property(p => p.UserId).IsRequired();

        builder.Property(p => p.Status).IsRequired();

        builder.HasOne(p => p.User)
            .WithMany(u => u.Posts)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(p => p.PostImages)
            .WithOne(pi => pi.Post)
            .HasForeignKey(pi => pi.PostId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(p => p.PostBrutals)
            .WithOne(pb => pb.Post)
            .HasForeignKey(pb => pb.PostId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(p => p.Likes)
            .WithOne(l => l.Post)
            .HasForeignKey(l => l.PostId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}