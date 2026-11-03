using ExpenseTracker.Api.Domain.Entities;
using ExpenseTracker.Api.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Api.Data;

public class DbSeeder(
    UserManager<ApplicationUser> userManager,
    ApplicationDbContext dbContext,
    ILogger<DbSeeder> logger
)
{
    public async Task SeedAsync()
    {
        try
        {
            if (dbContext.Database.IsRelational())
            {
                await dbContext.Database.MigrateAsync();
            }

            const string demoEmail = "demo@expensetracker.com";
            const string demoUsername = "demouser";

            var existingUser = await userManager.FindByEmailAsync(demoEmail);
            if (existingUser != null)
            {
                logger.LogInformation("Database already seeded with demo user.");
                return;
            }

            logger.LogInformation("Seeding database with demo data...");

            // 1. Create Demo User
            var user = new ApplicationUser
            {
                UserName = demoUsername,
                Email = demoEmail,
                FullName = "Demo User",
                EmailConfirmed = true,
                CreatedAtUtc = DateTime.UtcNow
            };

            var userResult = await userManager.CreateAsync(user, "Password123!");
            if (!userResult.Succeeded)
            {
                var errors = string.Join(", ", userResult.Errors.Select(e => e.Description));
                logger.LogError("Failed to seed demo user: {Errors}", errors);
                return;
            }

            // 2. Create Wallets
            var primaryWallet = new Wallet
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Name = "Rekening Utama (BCA)",
                Currency = "IDR",
                InitialBalance = 10_000_000m,
                Description = "Rekening operasional harian dan gaji",
                CreatedAtUtc = DateTime.UtcNow.AddDays(-60)
            };

            var cashWallet = new Wallet
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Name = "Dompet Tunai",
                Currency = "IDR",
                InitialBalance = 500_000m,
                Description = "Uang tunai fisik untuk kebutuhan sehari-hari",
                CreatedAtUtc = DateTime.UtcNow.AddDays(-60)
            };

            var savingsWallet = new Wallet
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Name = "Tabungan & Investasi",
                Currency = "IDR",
                InitialBalance = 25_000_000m,
                Description = "Dana darurat dan portofolio investasi",
                CreatedAtUtc = DateTime.UtcNow.AddDays(-60)
            };

            await dbContext.Wallets.AddRangeAsync(primaryWallet, cashWallet, savingsWallet);

            // 3. Create Categories for Primary Wallet
            var catSalary = new Category
            {
                Id = Guid.NewGuid(),
                WalletId = primaryWallet.Id,
                Name = "Gaji Bulanan",
                Type = CategoryType.Income,
                CreatedAtUtc = DateTime.UtcNow.AddDays(-60)
            };

            var catFreelance = new Category
            {
                Id = Guid.NewGuid(),
                WalletId = primaryWallet.Id,
                Name = "Freelance & Bonus",
                Type = CategoryType.Income,
                CreatedAtUtc = DateTime.UtcNow.AddDays(-60)
            };

            var catFoodPrimary = new Category
            {
                Id = Guid.NewGuid(),
                WalletId = primaryWallet.Id,
                Name = "Makanan & Minuman",
                Type = CategoryType.Expense,
                CreatedAtUtc = DateTime.UtcNow.AddDays(-60)
            };

            var catBills = new Category
            {
                Id = Guid.NewGuid(),
                WalletId = primaryWallet.Id,
                Name = "Tagihan & Utilitas",
                Type = CategoryType.Expense,
                CreatedAtUtc = DateTime.UtcNow.AddDays(-60)
            };

            var catGroceries = new Category
            {
                Id = Guid.NewGuid(),
                WalletId = primaryWallet.Id,
                Name = "Belanja Bulanan",
                Type = CategoryType.Expense,
                CreatedAtUtc = DateTime.UtcNow.AddDays(-60)
            };

            var catTransport = new Category
            {
                Id = Guid.NewGuid(),
                WalletId = primaryWallet.Id,
                Name = "Transportasi",
                Type = CategoryType.Expense,
                CreatedAtUtc = DateTime.UtcNow.AddDays(-60)
            };

            // Categories for Cash Wallet
            var catFoodCash = new Category
            {
                Id = Guid.NewGuid(),
                WalletId = cashWallet.Id,
                Name = "Jajan & Kopi",
                Type = CategoryType.Expense,
                CreatedAtUtc = DateTime.UtcNow.AddDays(-60)
            };

            var catParking = new Category
            {
                Id = Guid.NewGuid(),
                WalletId = cashWallet.Id,
                Name = "Parkir & Tol",
                Type = CategoryType.Expense,
                CreatedAtUtc = DateTime.UtcNow.AddDays(-60)
            };

            // Categories for Savings Wallet
            var catInvestmentIncome = new Category
            {
                Id = Guid.NewGuid(),
                WalletId = savingsWallet.Id,
                Name = "Bunga & Dividen",
                Type = CategoryType.Income,
                CreatedAtUtc = DateTime.UtcNow.AddDays(-60)
            };

            var catMutualFunds = new Category
            {
                Id = Guid.NewGuid(),
                WalletId = savingsWallet.Id,
                Name = "Investasi Reksadana",
                Type = CategoryType.Expense,
                CreatedAtUtc = DateTime.UtcNow.AddDays(-60)
            };

            await dbContext.Categories.AddRangeAsync(
                catSalary,
                catFreelance,
                catFoodPrimary,
                catBills,
                catGroceries,
                catTransport,
                catFoodCash,
                catParking,
                catInvestmentIncome,
                catMutualFunds
            );

            // 4. Create Sample Transactions
            var now = DateTime.UtcNow;

            var transactions = new List<Transaction>
            {
                // Incomes
                new()
                {
                    Id = Guid.NewGuid(),
                    CategoryId = catSalary.Id,
                    Amount = 15_000_000m,
                    TransactionDate = now.AddDays(-28),
                    Description = "Gaji Pokok Bulan Ini",
                    CreatedAtUtc = now.AddDays(-28)
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    CategoryId = catFreelance.Id,
                    Amount = 3_500_000m,
                    TransactionDate = now.AddDays(-15),
                    Description = "Pembayaran Project Desain Web",
                    CreatedAtUtc = now.AddDays(-15)
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    CategoryId = catInvestmentIncome.Id,
                    Amount = 250_000m,
                    TransactionDate = now.AddDays(-10),
                    Description = "Dividen Obligasi Pemerintah",
                    CreatedAtUtc = now.AddDays(-10)
                },

                // Expenses - Primary Wallet
                new()
                {
                    Id = Guid.NewGuid(),
                    CategoryId = catBills.Id,
                    Amount = 550_000m,
                    TransactionDate = now.AddDays(-25),
                    Description = "Listrik PLN & Tagihan Air",
                    CreatedAtUtc = now.AddDays(-25)
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    CategoryId = catBills.Id,
                    Amount = 400_000m,
                    TransactionDate = now.AddDays(-24),
                    Description = "Internet IndiHome 50 Mbps",
                    CreatedAtUtc = now.AddDays(-24)
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    CategoryId = catGroceries.Id,
                    Amount = 1_250_000m,
                    TransactionDate = now.AddDays(-20),
                    Description = "Belanja Bulanan Supermarket",
                    CreatedAtUtc = now.AddDays(-20)
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    CategoryId = catTransport.Id,
                    Amount = 250_000m,
                    TransactionDate = now.AddDays(-18),
                    Description = "Isi Bensin Pertamax",
                    CreatedAtUtc = now.AddDays(-18)
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    CategoryId = catFoodPrimary.Id,
                    Amount = 185_000m,
                    TransactionDate = now.AddDays(-12),
                    Description = "Makan Malam Bersama Keluarga",
                    CreatedAtUtc = now.AddDays(-12)
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    CategoryId = catMutualFunds.Id,
                    Amount = 2_000_000m,
                    TransactionDate = now.AddDays(-7),
                    Description = "Top Up Reksadana Saham Bibit",
                    CreatedAtUtc = now.AddDays(-7)
                },

                // Expenses - Cash Wallet
                new()
                {
                    Id = Guid.NewGuid(),
                    CategoryId = catFoodCash.Id,
                    Amount = 35_000m,
                    TransactionDate = now.AddDays(-5),
                    Description = "Kopi Susu Gula Aren",
                    CreatedAtUtc = now.AddDays(-5)
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    CategoryId = catFoodCash.Id,
                    Amount = 45_000m,
                    TransactionDate = now.AddDays(-3),
                    Description = "Makan Siang Nasi Padang",
                    CreatedAtUtc = now.AddDays(-3)
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    CategoryId = catParking.Id,
                    Amount = 10_000m,
                    TransactionDate = now.AddDays(-2),
                    Description = "Parkir Mall",
                    CreatedAtUtc = now.AddDays(-2)
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    CategoryId = catFoodCash.Id,
                    Amount = 28_000m,
                    TransactionDate = now.AddDays(-1),
                    Description = "Roti & Air Mineral Minimarket",
                    CreatedAtUtc = now.AddDays(-1)
                }
            };

            await dbContext.Transactions.AddRangeAsync(transactions);
            await dbContext.SaveChangesAsync();

            logger.LogInformation("Database seeded successfully with demo data.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }
}

