using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialApp.Domain.Entities;

namespace SocialApp.Data.Configurations;

public sealed class PostImageConfiguration : BaseEntityConfiguration<PostImage>
{
    public override void Configure(EntityTypeBuilder<PostImage> builder)
    {
        builder.ToTable("PostImages");

        builder.Property(pi => pi.File)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(pb => pb.PostId).IsRequired();

        builder.HasOne(pi => pi.Post)
            .WithMany(p => p.PostImages)
            .HasForeignKey(pi => pi.PostId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}