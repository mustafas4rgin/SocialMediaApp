using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialApp.Domain.Entities;

namespace SocialApp.Data.Configurations;

public sealed class UserConfiguration : BaseEntityConfiguration<User>
{
    public override void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        // Temel alan kısıtları
        builder.Property(u => u.Email)
         .IsRequired()
         .HasMaxLength(256);

        builder.Property(u => u.UserName)
         .IsRequired()
         .HasMaxLength(64);

        builder.Property(u => u.FirstName).HasMaxLength(64);
        builder.Property(u => u.LastName).HasMaxLength(64);

        builder.Property(u => u.PasswordHash).IsRequired();
        builder.Property(u => u.PasswordSalt).IsRequired();

        builder.HasIndex(u => u.Email).IsUnique();
        builder.HasIndex(u => u.UserName).IsUnique();

        builder.HasOne(u => u.Role)
         .WithMany(r => r.Users)
         .HasForeignKey("RoleId")          
         .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.UserImages)
         .WithOne(i => i.User)
         .HasForeignKey(i => i.UserId)
         .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.Posts)
         .WithOne(p => p.User)
         .HasForeignKey(p => p.UserId)
         .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.Comments)
         .WithOne(c => c.User)
         .HasForeignKey("UserId")
         .OnDelete(DeleteBehavior.Cascade);

    }
}