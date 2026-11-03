using ExpenseTracker.Api.Data;
using ExpenseTracker.Api.Domain.Entities;
using ExpenseTracker.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Api.Repositories.Implementations;

public class WalletRepository(
    ApplicationDbContext context
) : IWalletRepository
{
    public async Task<Wallet> CreateWalletAsync(Wallet wallet)
    {
        await context.Wallets.AddAsync(wallet);
        await context.SaveChangesAsync();

        return wallet;
    }

    public async Task<IReadOnlyList<Wallet>> GetWalletsAsync(string userId)
    {
        return await context.Wallets
            .AsNoTracking()
            .Where(w => w.UserId == userId)
            .Include(w => w.Categories)
            .ThenInclude(c => c.Transactions)
            .OrderByDescending(w => w.CreatedAtUtc)
            .ToListAsync();
    }

    public async Task<Wallet?> GetWalletAsync(string userId, Guid walletId)
    {
        return await context.Wallets
            .FirstOrDefaultAsync(w => w.Id == walletId && w.UserId == userId);
    }

    public async Task<Wallet?> GetWalletWithDetailsAsync(string userId, Guid walletId)
    {
        return await context.Wallets
            .AsNoTracking()
            .Include(w => w.Categories)
            .ThenInclude(c => c.Transactions)
            .FirstOrDefaultAsync(w => w.Id == walletId && w.UserId == userId);
    }

    public async Task UpdateWalletAsync(Wallet wallet)
    {
        context.Wallets.Update(wallet);
        await context.SaveChangesAsync();
    }

    public async Task DeleteWalletAsync(Wallet wallet)
    {
        context.Wallets.Remove(wallet);
        await context.SaveChangesAsync();
    }
}