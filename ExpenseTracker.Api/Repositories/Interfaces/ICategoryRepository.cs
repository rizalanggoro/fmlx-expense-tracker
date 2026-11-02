using ExpenseTracker.Api.Domain.Entities;

namespace ExpenseTracker.Api.Repositories.Interfaces;

public interface ICategoryRepository
{
    Task<IReadOnlyList<Category>> GetCategoriesByWalletIdAsync(Guid walletId);
    Task<Category?> GetCategoryWithWalletAsync(Guid categoryId);
    Task<bool> CategoryBelongsToUserAsync(Guid categoryId, string userId);
    Task<Category> AddAsync(Category category);
    Task UpdateAsync(Category category);
    Task DeleteAsync(Category category);
}
