using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaschengeldManager.Core.Entities;

namespace TaschengeldManager.Infrastructure.Data.Configurations;

public class TotpBackupCodeConfiguration : IEntityTypeConfiguration<TotpBackupCode>
{
    public void Configure(EntityTypeBuilder<TotpBackupCode> builder)
    {
        builder.ToTable("totp_backup_codes");

        builder.HasKey(bc => bc.Id);

        builder.Property(bc => bc.CodeHash)
            .IsRequired()
            .HasMaxLength(128);

        builder.HasIndex(bc => new { bc.UserId, bc.CodeHash });

        builder.HasOne(bc => bc.User)
            .WithMany(u => u.TotpBackupCodes)
            .HasForeignKey(bc => bc.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
