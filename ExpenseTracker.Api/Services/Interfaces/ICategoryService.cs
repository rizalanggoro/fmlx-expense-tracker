using ExpenseTracker.Api.Common;
using ExpenseTracker.Api.DTOs.Categories;

namespace ExpenseTracker.Api.Services.Interfaces;

public interface ICategoryService
{
    Task<ServiceResult<CategoryResponseDto>> CreateCategoryAsync(
        string userId,
        CreateCategoryDto request
    );

    Task<ServiceResult<IReadOnlyList<CategoryResponseDto>>> GetCategoriesAsync(
        string userId,
        Guid? walletId = null
    );

    Task<ServiceResult<CategoryResponseDto>> GetCategoryAsync(
        string userId,
        Guid categoryId
    );

    Task<ServiceResult<CategoryResponseDto>> UpdateCategoryAsync(
        string userId,
        Guid categoryId,
        UpdateCategoryDto request
    );

    Task<ServiceResult<bool>> DeleteCategoryAsync(
        string userId,
        Guid categoryId
    );
}