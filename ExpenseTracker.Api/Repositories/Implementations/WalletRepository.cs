using ExpenseTracker.Api.Data;
using ExpenseTracker.Api.Domain.Entities;
using ExpenseTracker.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Api.Repositories.Implementations;

public class WalletRepository(
    ApplicationDbContext context
) : IWalletRepository
{
    public async Task<Wallet> CreateAsync(Wallet wallet)
    {
        await context.Wallets.AddAsync(wallet);
        await context.SaveChangesAsync();

        return wallet;
    }

    public async Task<IReadOnlyList<Wallet>> GetAllAsync(string userId)
    {
        return await context.Wallets
            .AsNoTracking()
            .Where(w => w.UserId == userId)
            .Include(w => w.Categories)
            .ThenInclude(c => c.Transactions)
            .OrderByDescending(w => w.CreatedAtUtc)
            .ToListAsync();
    }

    public async Task<Wallet?> GetAsync(string userId, Guid walletId)
    {
        return await context.Wallets
            .FirstOrDefaultAsync(w => w.Id == walletId && w.UserId == userId);
    }

    public async Task<Wallet?> GetWithDetailsAsync(string userId, Guid walletId)
    {
        return await context.Wallets
            .AsNoTracking()
            .Include(w => w.Categories)
            .ThenInclude(c => c.Transactions)
            .FirstOrDefaultAsync(w => w.Id == walletId && w.UserId == userId);
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