using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialApp.Domain.Entities;

namespace SocialApp.Data.Configurations;

public sealed class CommentResponseConfiguration : BaseEntityConfiguration<CommentResponse>
{
    public override void Configure(EntityTypeBuilder<CommentResponse> builder)
    {
        builder.ToTable("Responses");

        builder.Property(cr => cr.CommentId).IsRequired();

        builder.HasOne(cr => cr.Comment)
            .WithMany(c => c.Responses)
            .HasForeignKey(cr => cr.CommentId)
            .OnDelete(DeleteBehavior.NoAction);
        
    }
}