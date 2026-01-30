using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaschengeldManager.Core.Entities;

namespace TaschengeldManager.Infrastructure.Data.Configurations;

public class RecurringPaymentConfiguration : IEntityTypeConfiguration<RecurringPayment>
{
    public void Configure(EntityTypeBuilder<RecurringPayment> builder)
    {
        builder.ToTable("recurring_payments");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Amount)
            .HasPrecision(18, 2);

        builder.Property(r => r.Description)
            .HasMaxLength(200);

        builder.HasIndex(r => r.AccountId);
        builder.HasIndex(r => r.CreatedByUserId);
        builder.HasIndex(r => r.NextExecutionDate);
        builder.HasIndex(r => new { r.IsActive, r.NextExecutionDate });

        builder.HasOne(r => r.Account)
            .WithMany()
            .HasForeignKey(r => r.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.CreatedByUser)
            .WithMany()
            .HasForeignKey(r => r.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
