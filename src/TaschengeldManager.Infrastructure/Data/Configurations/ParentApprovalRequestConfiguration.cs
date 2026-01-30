using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaschengeldManager.Core.Entities;

namespace TaschengeldManager.Infrastructure.Data.Configurations;

public class ParentApprovalRequestConfiguration : IEntityTypeConfiguration<ParentApprovalRequest>
{
    public void Configure(EntityTypeBuilder<ParentApprovalRequest> builder)
    {
        builder.ToTable("parent_approval_requests");

        builder.HasKey(par => par.Id);

        builder.Property(par => par.DeviceInfo)
            .HasMaxLength(500);

        builder.Property(par => par.ApproximateLocation)
            .HasMaxLength(100);

        builder.Property(par => par.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.HasIndex(par => par.ChildUserId);
        builder.HasIndex(par => par.FamilyId);
        builder.HasIndex(par => par.ExpiresAt);

        builder.HasOne(par => par.ChildUser)
            .WithMany()
            .HasForeignKey(par => par.ChildUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(par => par.Family)
            .WithMany()
            .HasForeignKey(par => par.FamilyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(par => par.RespondedByUser)
            .WithMany()
            .HasForeignKey(par => par.RespondedByUserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
