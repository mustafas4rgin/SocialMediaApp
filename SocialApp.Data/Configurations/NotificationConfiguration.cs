using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialApp.Domain.Entities;

namespace SocialApp.Data.Configurations;

public sealed class NotificationConfiguration : BaseEntityConfiguration<Notification>
{
    public override void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications");

        builder.Property(n => n.IsSeen)
            .HasDefaultValue(false);

        builder.Property(n => n.UserId)
            .IsRequired();
        
        builder.Property(n => n.Message)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasOne(n => n.User)
            .WithMany(u => u.Notifications)
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}