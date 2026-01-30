using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaschengeldManager.Core.Entities;

namespace TaschengeldManager.Infrastructure.Data.Configurations;

public class FamilyParentConfiguration : IEntityTypeConfiguration<FamilyParent>
{
    public void Configure(EntityTypeBuilder<FamilyParent> builder)
    {
        builder.ToTable("family_parents");

        builder.HasKey(fp => new { fp.FamilyId, fp.UserId });

        builder.HasOne(fp => fp.Family)
            .WithMany(f => f.Parents)
            .HasForeignKey(fp => fp.FamilyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(fp => fp.User)
            .WithMany(u => u.ParentFamilies)
            .HasForeignKey(fp => fp.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
