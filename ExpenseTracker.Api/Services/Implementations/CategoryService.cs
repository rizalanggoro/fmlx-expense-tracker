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
    public async Task<ServiceResult<CategoryResponseDto>> CreateAsync(string userId, CreateCategoryDto request)
    {
        var wallet = await walletRepository.GetAsync(userId, request.WalletId);
        if (wallet == null)
            return ServiceResult<CategoryResponseDto>.Failure(
                "Wallet not found.",
                404
            );

        var category = mapper.Map<Category>(request);
        category.Id = Guid.NewGuid();
        category.CreatedAtUtc = DateTime.UtcNow;

        await categoryRepository.CreateAsync(category);
        var response = mapper.Map<CategoryResponseDto>(category);

        return ServiceResult<CategoryResponseDto>.Success(
            response,
            "Category created successfully.",
            201
        );
    }

    public async Task<ServiceResult<IReadOnlyList<CategoryResponseDto>>> GetAllAsync(string userId,
        Guid? walletId = null)
    {
        if (walletId.HasValue)
        {
            var wallet = await walletRepository.GetAsync(userId, walletId.Value);
            if (wallet == null)
                return ServiceResult<IReadOnlyList<CategoryResponseDto>>.Failure(
                    "Wallet not found.",
                    404
                );
        }

        var categories = await categoryRepository.GetAllAsync(userId, walletId);
        var response = mapper.Map<IReadOnlyList<CategoryResponseDto>>(categories);

        return ServiceResult<IReadOnlyList<CategoryResponseDto>>.Success(response);
    }

    public async Task<ServiceResult<CategoryResponseDto>> GetAsync(string userId, Guid categoryId)
    {
        var category = await categoryRepository.GetAsync(userId, categoryId);
        if (category == null)
            return ServiceResult<CategoryResponseDto>.Failure(
                "Category not found.",
                404
            );

        var response = mapper.Map<CategoryResponseDto>(category);

        return ServiceResult<CategoryResponseDto>.Success(response);
    }

    public async Task<ServiceResult<CategoryResponseDto>> UpdateAsync(string userId, Guid categoryId,
        UpdateCategoryDto request)
    {
        var category = await categoryRepository.GetAsync(userId, categoryId);
        if (category == null)
            return ServiceResult<CategoryResponseDto>.Failure(
                "Category not found.",
                404
            );

        mapper.Map(request, category);
        category.UpdatedAtUtc = DateTime.UtcNow;

        await categoryRepository.UpdateAsync(category);

        var response = mapper.Map<CategoryResponseDto>(category);

        return ServiceResult<CategoryResponseDto>.Success(
            response,
            "Category updated successfully."
        );
    }

    public async Task<ServiceResult<bool>> DeleteAsync(string userId, Guid categoryId)
    {
        var category = await categoryRepository.GetAsync(userId, categoryId);
        if (category == null)
            return ServiceResult<bool>.Failure(
                "Category not found.",
                404
            );

        await categoryRepository.DeleteAsync(category);

        return ServiceResult<bool>.Success(
            true,
            "Category deleted successfully."
        );
    }
}