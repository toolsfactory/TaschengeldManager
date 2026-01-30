using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaschengeldManager.Core.Entities;

namespace TaschengeldManager.Infrastructure.Data.Configurations;

public class MoneyRequestConfiguration : IEntityTypeConfiguration<MoneyRequest>
{
    public void Configure(EntityTypeBuilder<MoneyRequest> builder)
    {
        builder.ToTable("money_requests");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Amount)
            .HasPrecision(18, 2);

        builder.Property(r => r.Reason)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(r => r.ResponseNote)
            .HasMaxLength(500);

        builder.HasIndex(r => r.ChildUserId);
        builder.HasIndex(r => r.Status);
        builder.HasIndex(r => new { r.ChildUserId, r.Status });

        builder.HasOne(r => r.ChildUser)
            .WithMany()
            .HasForeignKey(r => r.ChildUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.RespondedByUser)
            .WithMany()
            .HasForeignKey(r => r.RespondedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.ResultingTransaction)
            .WithMany()
            .HasForeignKey(r => r.ResultingTransactionId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
