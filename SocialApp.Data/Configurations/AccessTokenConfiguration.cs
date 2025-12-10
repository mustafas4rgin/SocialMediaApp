using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialApp.Domain.Entities;

namespace SocialApp.Data.Configurations;

public sealed class AccesTokenConfiguration : BaseEntityConfiguration<AccessToken>
{
    public override void Configure(EntityTypeBuilder<AccessToken> builder)
    {
        builder.ToTable("AccessTokens");

        builder.HasOne(at => at.User)
           .WithMany()
           .HasForeignKey(at => at.UserId)
           .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(at => at.Token).IsUnique();
        builder.HasIndex(at => at.UserId);
        builder.HasIndex(at => at.ExpiresAt);
    }
}