using ExpenseTracker.Api.Data;
using ExpenseTracker.Api.Domain.Entities;
using ExpenseTracker.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Api.Repositories.Implementations;

public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
{
    public CategoryRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Category>> GetCategoriesByWalletIdAsync(Guid walletId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(c => c.WalletId == walletId)
            .Include(c => c.Transactions)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<Category?> GetCategoryWithWalletAsync(Guid categoryId)
    {
        return await _dbSet
            .Include(c => c.Wallet)
            .FirstOrDefaultAsync(c => c.Id == categoryId);
    }

    public async Task<bool> CategoryBelongsToUserAsync(Guid categoryId, string userId)
    {
        return await _dbSet
            .AsNoTracking()
            .AnyAsync(c => c.Id == categoryId && c.Wallet.UserId == userId);
    }
}

