using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialApp.Domain.Entities;

namespace SocialApp.Data.Configurations;

public sealed class UserImageConfiguration : BaseEntityConfiguration<UserImage>
{
    public override void Configure(EntityTypeBuilder<UserImage> builder)
    {
        builder.ToTable("UserImages");

        builder.Property(ui => ui.UserId).IsRequired();

        builder.Property(ui => ui.File)
            .IsRequired()
            .HasMaxLength(128);
    }
}