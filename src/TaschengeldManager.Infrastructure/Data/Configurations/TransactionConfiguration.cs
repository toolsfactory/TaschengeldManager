using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaschengeldManager.Core.Entities;

namespace TaschengeldManager.Infrastructure.Data.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("transactions");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Amount)
            .HasPrecision(18, 2);

        builder.Property(t => t.BalanceAfter)
            .HasPrecision(18, 2);

        builder.Property(t => t.Type)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(t => t.Description)
            .HasMaxLength(200);

        builder.Property(t => t.Category)
            .HasMaxLength(50);

        builder.HasIndex(t => t.AccountId);
        builder.HasIndex(t => t.CreatedAt);

        // Composite index for efficient transaction queries by account
        builder.HasIndex(t => new { t.AccountId, t.CreatedAt })
            .IsDescending(false, true)
            .HasDatabaseName("IX_transactions_AccountId_CreatedAt_Desc");

        // Index for filtering by transaction type
        builder.HasIndex(t => new { t.CreatedByUserId, t.Type })
            .HasDatabaseName("IX_transactions_CreatedByUserId_Type");

        builder.HasOne(t => t.Account)
            .WithMany(a => a.Transactions)
            .HasForeignKey(t => t.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.CreatedByUser)
            .WithMany()
            .HasForeignKey(t => t.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
