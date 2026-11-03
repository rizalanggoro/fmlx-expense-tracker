using ExpenseTracker.Api.Domain.Entities;

namespace ExpenseTracker.Api.Repositories.Interfaces;

public interface ICategoryRepository
{
    Task<Category> CreateCategoryAsync(Category category);
    Task<IReadOnlyList<Category>> GetCategoriesAsync(string userId, Guid? walletId = null);
    Task<Category?> GetCategoryAsync(string userId, Guid categoryId);
    Task UpdateCategoryAsync(Category category);
    Task DeleteCategoryAsync(Category category);
}