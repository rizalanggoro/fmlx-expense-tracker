using ExpenseTracker.Api.Common;
using ExpenseTracker.Api.DTOs.Categories;

namespace ExpenseTracker.Api.Services.Interfaces;

public interface ICategoryService
{
    Task<ServiceResult<CategoryResponseDto>> CreateAsync(
        string userId,
        CreateCategoryDto request
    );

    Task<ServiceResult<IReadOnlyList<CategoryResponseDto>>> GetAllAsync(
        string userId,
        Guid? walletId = null
    );

    Task<ServiceResult<CategoryResponseDto>> GetAsync(
        string userId,
        Guid categoryId
    );

    Task<ServiceResult<CategoryResponseDto>> UpdateAsync(
        string userId,
        Guid categoryId,
        UpdateCategoryDto request
    );

    Task<ServiceResult<bool>> DeleteAsync(
        string userId,
        Guid categoryId
    );
}