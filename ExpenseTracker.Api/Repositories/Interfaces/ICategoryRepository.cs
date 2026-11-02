using ExpenseTracker.Api.Domain.Entities;

namespace ExpenseTracker.Api.Repositories.Interfaces;

public interface ICategoryRepository : IGenericRepository<Category>
{
    Task<IReadOnlyList<Category>> GetCategoriesByWalletIdAsync(Guid walletId);
    Task<Category?> GetCategoryWithWalletAsync(Guid categoryId);
    Task<bool> CategoryBelongsToUserAsync(Guid categoryId, string userId);
}
