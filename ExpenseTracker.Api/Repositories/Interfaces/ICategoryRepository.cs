using ExpenseTracker.Api.Domain.Entities;

namespace ExpenseTracker.Api.Repositories.Interfaces;

public interface ICategoryRepository
{
    Task<Category> CreateAsync(Category category);
    Task<IReadOnlyList<Category>> GetAllAsync(string userId, Guid? walletId = null);
    Task<Category?> GetAsync(string userId, Guid categoryId);
    Task UpdateAsync(Category category);
    Task DeleteAsync(Category category);
}