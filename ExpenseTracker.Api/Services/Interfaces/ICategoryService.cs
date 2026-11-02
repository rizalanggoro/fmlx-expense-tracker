using ExpenseTracker.Api.Common;
using ExpenseTracker.Api.DTOs.Categories;

namespace ExpenseTracker.Api.Services.Interfaces;

public interface ICategoryService
{
    Task<ServiceResult<IReadOnlyList<CategoryResponseDto>>> GetCategoriesAsync(string userId, Guid? walletId = null);
    Task<ServiceResult<CategoryResponseDto>> GetCategoryByIdAsync(Guid categoryId, string userId);
    Task<ServiceResult<CategoryResponseDto>> CreateCategoryAsync(string userId, CreateCategoryDto request);
    Task<ServiceResult<CategoryResponseDto>> UpdateCategoryAsync(Guid categoryId, string userId, UpdateCategoryDto request);
    Task<ServiceResult<bool>> DeleteCategoryAsync(Guid categoryId, string userId);
}
