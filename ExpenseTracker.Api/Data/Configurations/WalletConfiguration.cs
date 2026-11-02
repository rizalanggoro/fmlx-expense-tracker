using ExpenseTracker.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseTracker.Api.Data.Configurations;

public class WalletConfiguration : IEntityTypeConfiguration<Wallet>
{
    public void Configure(EntityTypeBuilder<Wallet> builder)
    {
        builder.ToTable("Wallets");

        builder.HasKey(w => w.Id);

        builder.Property(w => w.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(w => w.Currency)
            .IsRequired()
            .HasMaxLength(3)
            .HasDefaultValue("IDR");

        builder.Property(w => w.InitialBalance)
            .HasColumnType("decimal(18,2)")
            .HasDefaultValue(0.00m);

        builder.Property(w => w.Description)
            .HasMaxLength(255);

        builder.Property(w => w.CreatedAtUtc)
            .IsRequired();

        builder.HasOne(w => w.User)
            .WithMany(u => u.Wallets)
            .HasForeignKey(w => w.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(w => w.Categories)
            .WithOne(c => c.Wallet)
            .HasForeignKey(c => c.WalletId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(w => new { w.UserId, w.Name });
    }
}
