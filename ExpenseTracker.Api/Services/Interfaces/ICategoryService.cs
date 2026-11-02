using ExpenseTracker.Api.Common;
using ExpenseTracker.Api.DTOs.Categories;

namespace ExpenseTracker.Api.Services.Interfaces;

public interface ICategoryService
{
    Task<ServiceResult<IReadOnlyList<CategoryResponseDto>>> GetCategoriesByWalletIdAsync(Guid walletId, string userId);
    Task<ServiceResult<CategoryResponseDto>> GetCategoryByIdAsync(Guid categoryId, string userId);
    Task<ServiceResult<CategoryResponseDto>> CreateCategoryAsync(Guid walletId, string userId, CreateCategoryDto request);
    Task<ServiceResult<CategoryResponseDto>> UpdateCategoryAsync(Guid categoryId, string userId, UpdateCategoryDto request);
    Task<ServiceResult<bool>> DeleteCategoryAsync(Guid categoryId, string userId);
}
