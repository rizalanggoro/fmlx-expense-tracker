using ExpenseTracker.Api.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Api.Data;

public class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options
) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Wallet> Wallets => Set<Wallet>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // ApplicationUser configuration
        builder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(u => u.FullName).HasMaxLength(100);
            entity.Property(u => u.CreatedAtUtc).IsRequired();

            entity.HasMany(u => u.Wallets)
                .WithOne(w => w.User)
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Wallet configuration
        builder.Entity<Wallet>(entity =>
        {
            entity.ToTable("Wallets");
            entity.HasKey(w => w.Id);
            entity.Property(w => w.Name).IsRequired().HasMaxLength(100);
            entity.Property(w => w.Currency).IsRequired().HasMaxLength(3).HasDefaultValue("IDR");
            entity.Property(w => w.InitialBalance).HasColumnType("decimal(18,2)").HasDefaultValue(0.00m);
            entity.Property(w => w.Description).HasMaxLength(255);
            entity.Property(w => w.CreatedAtUtc).IsRequired();

            entity.HasOne(w => w.User)
                .WithMany(u => u.Wallets)
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(w => w.Categories)
                .WithOne(c => c.Wallet)
                .HasForeignKey(c => c.WalletId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(w => new { w.UserId, w.Name });
        });

        // Category configuration
        builder.Entity<Category>(entity =>
        {
            entity.ToTable("Categories");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Name).IsRequired().HasMaxLength(100);
            entity.Property(c => c.Type).IsRequired().HasConversion<int>();
            entity.Property(c => c.CreatedAtUtc).IsRequired();

            entity.HasOne(c => c.Wallet)
                .WithMany(w => w.Categories)
                .HasForeignKey(c => c.WalletId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(c => c.Transactions)
                .WithOne(t => t.Category)
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(c => new { c.WalletId, c.Name });
        });

        // Transaction configuration
        builder.Entity<Transaction>(entity =>
        {
            entity.ToTable("Transactions");
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Amount).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(t => t.TransactionDate).IsRequired();
            entity.Property(t => t.Description).HasMaxLength(500);
            entity.Property(t => t.CreatedAtUtc).IsRequired();

            entity.HasOne(t => t.Category)
                .WithMany(c => c.Transactions)
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(t => new { t.CategoryId, t.TransactionDate });
        });
    }
}
