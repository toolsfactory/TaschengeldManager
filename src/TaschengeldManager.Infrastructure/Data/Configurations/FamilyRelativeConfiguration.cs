using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaschengeldManager.Core.Entities;

namespace TaschengeldManager.Infrastructure.Data.Configurations;

public class FamilyRelativeConfiguration : IEntityTypeConfiguration<FamilyRelative>
{
    public void Configure(EntityTypeBuilder<FamilyRelative> builder)
    {
        builder.ToTable("family_relatives");

        builder.HasKey(fr => new { fr.FamilyId, fr.UserId });

        builder.Property(fr => fr.RelationshipDescription)
            .HasMaxLength(50);

        builder.HasOne(fr => fr.Family)
            .WithMany(f => f.Relatives)
            .HasForeignKey(fr => fr.FamilyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(fr => fr.User)
            .WithMany(u => u.RelativeFamilies)
            .HasForeignKey(fr => fr.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
