using ExpenseTracker.Api.Data;
using ExpenseTracker.Api.Domain.Entities;
using ExpenseTracker.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Api.Repositories.Implementations;

public class WalletRepository : GenericRepository<Wallet>, IWalletRepository
{
    public WalletRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Wallet>> GetWalletsByUserIdAsync(string userId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(w => w.UserId == userId)
            .Include(w => w.Categories)
                .ThenInclude(c => c.Transactions)
            .OrderByDescending(w => w.CreatedAtUtc)
            .ToListAsync();
    }

    public async Task<Wallet?> GetWalletWithDetailsAsync(Guid walletId, string userId)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(w => w.Categories)
                .ThenInclude(c => c.Transactions)
            .FirstOrDefaultAsync(w => w.Id == walletId && w.UserId == userId);
    }

    public async Task<bool> ExistsAsync(Guid walletId, string userId)
    {
        return await _dbSet
            .AsNoTracking()
            .AnyAsync(w => w.Id == walletId && w.UserId == userId);
    }
}

