using AutoMapper;
using ExpenseTracker.Api.Common;
using ExpenseTracker.Api.Domain.Entities;
using ExpenseTracker.Api.DTOs.Categories;
using ExpenseTracker.Api.Repositories.Interfaces;
using ExpenseTracker.Api.Services.Interfaces;

namespace ExpenseTracker.Api.Services.Implementations;

public class CategoryService(
    ICategoryRepository categoryRepository,
    IWalletRepository walletRepository,
    IMapper mapper
) : ICategoryService
{
    public async Task<ServiceResult<CategoryResponseDto>> CreateCategoryAsync(string userId, CreateCategoryDto request)
    {
        var walletExists = await walletRepository.ExistsAsync(request.WalletId, userId);
        if (!walletExists)
            return ServiceResult<CategoryResponseDto>.NotFound("Wallet not found.");

        var category = mapper.Map<Category>(request);
        category.Id = Guid.NewGuid();
        category.CreatedAtUtc = DateTime.UtcNow;

        await categoryRepository.AddAsync(category);

        var response = mapper.Map<CategoryResponseDto>(category);

        return ServiceResult<CategoryResponseDto>.Success(
            response,
            "Category created successfully.",
            201
        );
    }

    public async Task<ServiceResult<IReadOnlyList<CategoryResponseDto>>> GetCategoriesAsync(string userId,
        Guid? walletId = null)
    {
        if (walletId.HasValue)
        {
            var walletExists = await walletRepository.ExistsAsync(walletId.Value, userId);
            if (!walletExists)
                return ServiceResult<IReadOnlyList<CategoryResponseDto>>.NotFound("Wallet not found.");
        }

        var categories = await categoryRepository.GetCategoriesAsync(userId, walletId);
        var response = mapper.Map<IReadOnlyList<CategoryResponseDto>>(categories);

        return ServiceResult<IReadOnlyList<CategoryResponseDto>>.Success(response);
    }

    public async Task<ServiceResult<CategoryResponseDto>> GetCategoryByIdAsync(Guid categoryId, string userId)
    {
        var category = await categoryRepository.GetCategoryWithWalletAsync(categoryId);
        if (category == null || category.Wallet.UserId != userId)
            return ServiceResult<CategoryResponseDto>.NotFound("Category not found.");

        var response = mapper.Map<CategoryResponseDto>(category);

        return ServiceResult<CategoryResponseDto>.Success(response);
    }

    public async Task<ServiceResult<CategoryResponseDto>> UpdateCategoryAsync(Guid categoryId, string userId,
        UpdateCategoryDto request)
    {
        var category = await categoryRepository.GetCategoryWithWalletAsync(categoryId);
        if (category == null || category.Wallet.UserId != userId)
            return ServiceResult<CategoryResponseDto>.NotFound("Category not found.");

        mapper.Map(request, category);
        category.UpdatedAtUtc = DateTime.UtcNow;

        await categoryRepository.UpdateAsync(category);

        var response = mapper.Map<CategoryResponseDto>(category);

        return ServiceResult<CategoryResponseDto>.Success(
            response,
            "Category updated successfully."
        );
    }

    public async Task<ServiceResult<bool>> DeleteCategoryAsync(Guid categoryId, string userId)
    {
        var category = await categoryRepository.GetCategoryWithWalletAsync(categoryId);
        if (category == null || category.Wallet.UserId != userId)
            return ServiceResult<bool>.NotFound("Category not found.");

        await categoryRepository.DeleteAsync(category);

        return ServiceResult<bool>.Success(
            true,
            "Category deleted successfully."
        );
    }
}