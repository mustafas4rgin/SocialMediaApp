using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialApp.Domain.Entities;

namespace SocialApp.Data.Configurations;

public sealed class FollowConfiguration : BaseEntityConfiguration<Follow>
{
    public override void Configure(EntityTypeBuilder<Follow> builder)
    {
         builder.ToTable("Follows");

        builder.Property(f => f.FollowerId).IsRequired();
        builder.Property(f => f.FollowingId).IsRequired();

        builder.HasIndex(f => new { f.FollowerId, f.FollowingId }).IsUnique();

        builder.HasIndex(f => f.FollowerId);
        builder.HasIndex(f => f.FollowingId);

        builder.HasOne(f => f.Follower)
         .WithMany(u => u.Followings)
         .HasForeignKey(f => f.FollowerId)
         .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(f => f.Following)
         .WithMany(u => u.Followers)
         .HasForeignKey(f => f.FollowingId)
         .OnDelete(DeleteBehavior.Restrict);
    }
}