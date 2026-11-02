using ExpenseTracker.Api.Data;
using ExpenseTracker.Api.Domain.Entities;
using ExpenseTracker.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Api.Repositories.Implementations;

public class CategoryRepository(ApplicationDbContext context) : ICategoryRepository
{
    public async Task<IReadOnlyList<Category>> GetCategoriesAsync(string userId, Guid? walletId = null)
    {
        var query = context.Categories
            .AsNoTracking()
            .Where(c => c.Wallet.UserId == userId);

        if (walletId.HasValue)
        {
            query = query.Where(c => c.WalletId == walletId.Value);
        }

        return await query
            .Include(c => c.Transactions)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Category>> GetCategoriesByWalletIdAsync(Guid walletId)
    {
        return await context.Categories
            .AsNoTracking()
            .Where(c => c.WalletId == walletId)
            .Include(c => c.Transactions)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<Category?> GetCategoryWithWalletAsync(Guid categoryId)
    {
        return await context.Categories
            .Include(c => c.Wallet)
            .FirstOrDefaultAsync(c => c.Id == categoryId);
    }

    public async Task<bool> CategoryBelongsToUserAsync(Guid categoryId, string userId)
    {
        return await context.Categories
            .AsNoTracking()
            .AnyAsync(c => c.Id == categoryId && c.Wallet.UserId == userId);
    }

    public async Task<Category> AddAsync(Category category)
    {
        await context.Categories.AddAsync(category);
        await context.SaveChangesAsync();
        return category;
    }

    public async Task UpdateAsync(Category category)
    {
        context.Categories.Update(category);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Category category)
    {
        context.Categories.Remove(category);
        await context.SaveChangesAsync();
    }
}
