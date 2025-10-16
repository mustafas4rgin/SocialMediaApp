using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialApp.Domain.Entities;

namespace SocialApp.Data.Configurations;

public sealed class PostBrutalConfiguration : BaseEntityConfiguration<PostBrutal>
{
    public override void Configure(EntityTypeBuilder<PostBrutal> builder)
    {
        builder.ToTable("PostBrutals");

        builder.Property(pb => pb.File)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(pb => pb.PostId).IsRequired();

        builder.HasOne(pb => pb.Post)
            .WithMany(p => p.PostBrutals)
            .HasForeignKey(pb => pb.PostId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}