using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaschengeldManager.Core.Entities;

namespace TaschengeldManager.Infrastructure.Data.Configurations;

public class FamilyInvitationConfiguration : IEntityTypeConfiguration<FamilyInvitation>
{
    public void Configure(EntityTypeBuilder<FamilyInvitation> builder)
    {
        builder.ToTable("family_invitations");

        builder.HasKey(fi => fi.Id);

        builder.Property(fi => fi.InvitedEmail)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(fi => fi.NormalizedInvitedEmail)
            .IsRequired()
            .HasMaxLength(256);

        builder.HasIndex(fi => fi.NormalizedInvitedEmail);

        builder.Property(fi => fi.Token)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(fi => fi.TokenHash)
            .IsRequired()
            .HasMaxLength(128);

        builder.HasIndex(fi => fi.TokenHash)
            .IsUnique();

        builder.Property(fi => fi.InvitedRole)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(fi => fi.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(fi => fi.RelationshipDescription)
            .HasMaxLength(50);

        builder.HasOne(fi => fi.Family)
            .WithMany(f => f.Invitations)
            .HasForeignKey(fi => fi.FamilyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(fi => fi.InvitedByUser)
            .WithMany()
            .HasForeignKey(fi => fi.InvitedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
