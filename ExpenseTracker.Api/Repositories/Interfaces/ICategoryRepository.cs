using ExpenseTracker.Api.Domain.Entities;

namespace ExpenseTracker.Api.Repositories.Interfaces;

public interface ICategoryRepository
{
    Task<IReadOnlyList<Category>> GetCategoriesAsync(string userId, Guid? walletId = null);
    Task<Category?> GetCategoryWithWalletAsync(Guid categoryId);
    Task<bool> CategoryBelongsToUserAsync(Guid categoryId, string userId);
    Task<Category> AddAsync(Category category);
    Task UpdateAsync(Category category);
    Task DeleteAsync(Category category);
}
