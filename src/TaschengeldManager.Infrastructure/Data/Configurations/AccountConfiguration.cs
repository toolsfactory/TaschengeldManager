using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaschengeldManager.Core.Entities;

namespace TaschengeldManager.Infrastructure.Data.Configurations;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("accounts");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Balance)
            .HasPrecision(18, 2);

        builder.Property(a => a.InterestRate)
            .HasPrecision(5, 2);

        builder.Property(a => a.InterestInterval)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.HasIndex(a => a.UserId)
            .IsUnique();

        // Index for interest calculation queries
        builder.HasIndex(a => a.InterestEnabled)
            .HasFilter("\"InterestEnabled\" = true")
            .HasDatabaseName("IX_accounts_InterestEnabled_Filtered");
    }
}
