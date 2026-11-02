using ExpenseTracker.Api.Data;
using ExpenseTracker.Api.Domain.Entities;
using ExpenseTracker.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Api.Repositories.Implementations;

public class WalletRepository(ApplicationDbContext context) : IWalletRepository
{
    public async Task<IReadOnlyList<Wallet>> GetWalletsByUserIdAsync(string userId)
    {
        return await context.Wallets
            .AsNoTracking()
            .Where(w => w.UserId == userId)
            .Include(w => w.Categories)
            .ThenInclude(c => c.Transactions)
            .OrderByDescending(w => w.CreatedAtUtc)
            .ToListAsync();
    }

    public async Task<Wallet?> GetWalletWithDetailsAsync(Guid walletId, string userId)
    {
        return await context.Wallets
            .AsNoTracking()
            .Include(w => w.Categories)
            .ThenInclude(c => c.Transactions)
            .FirstOrDefaultAsync(w => w.Id == walletId && w.UserId == userId);
    }

    public async Task<bool> ExistsAsync(Guid walletId, string userId)
    {
        return await context.Wallets
            .AsNoTracking()
            .AnyAsync(w => w.Id == walletId && w.UserId == userId);
    }

    public async Task<Wallet> AddAsync(Wallet wallet)
    {
        await context.Wallets.AddAsync(wallet);
        await context.SaveChangesAsync();
        return wallet;
    }

    public async Task UpdateAsync(Wallet wallet)
    {
        context.Wallets.Update(wallet);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Wallet wallet)
    {
        context.Wallets.Remove(wallet);
        await context.SaveChangesAsync();
    }
}