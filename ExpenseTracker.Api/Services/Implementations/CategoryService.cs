using AutoMapper;
using ExpenseTracker.Api.Common;
using ExpenseTracker.Api.Domain.Entities;
using ExpenseTracker.Api.DTOs.Categories;
using ExpenseTracker.Api.Repositories.Interfaces;
using ExpenseTracker.Api.Services.Interfaces;

namespace ExpenseTracker.Api.Services.Implementations;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ServiceResult<IReadOnlyList<CategoryResponseDto>>> GetCategoriesByWalletIdAsync(Guid walletId, string userId)
    {
        var walletExists = await _unitOfWork.Wallets.ExistsAsync(walletId, userId);
        if (!walletExists)
        {
            return ServiceResult<IReadOnlyList<CategoryResponseDto>>.NotFound("Wallet not found.");
        }

        var categories = await _unitOfWork.Categories.GetCategoriesByWalletIdAsync(walletId);
        var response = _mapper.Map<IReadOnlyList<CategoryResponseDto>>(categories);
        return ServiceResult<IReadOnlyList<CategoryResponseDto>>.Success(response);
    }

    public async Task<ServiceResult<CategoryResponseDto>> GetCategoryByIdAsync(Guid categoryId, string userId)
    {
        var category = await _unitOfWork.Categories.GetCategoryWithWalletAsync(categoryId);
        if (category == null || category.Wallet.UserId != userId)
        {
            return ServiceResult<CategoryResponseDto>.NotFound("Category not found.");
        }

        var response = _mapper.Map<CategoryResponseDto>(category);
        return ServiceResult<CategoryResponseDto>.Success(response);
    }

    public async Task<ServiceResult<CategoryResponseDto>> CreateCategoryAsync(Guid walletId, string userId, CreateCategoryDto request)
    {
        var walletExists = await _unitOfWork.Wallets.ExistsAsync(walletId, userId);
        if (!walletExists)
        {
            return ServiceResult<CategoryResponseDto>.NotFound("Wallet not found.");
        }

        var category = _mapper.Map<Category>(request);
        category.Id = Guid.NewGuid();
        category.WalletId = walletId;
        category.CreatedAtUtc = DateTime.UtcNow;

        await _unitOfWork.Categories.AddAsync(category);
        await _unitOfWork.SaveChangesAsync();

        var response = _mapper.Map<CategoryResponseDto>(category);
        return ServiceResult<CategoryResponseDto>.Success(response, "Category created successfully.", statusCode: 201);
    }

    public async Task<ServiceResult<CategoryResponseDto>> UpdateCategoryAsync(Guid categoryId, string userId, UpdateCategoryDto request)
    {
        var category = await _unitOfWork.Categories.GetCategoryWithWalletAsync(categoryId);
        if (category == null || category.Wallet.UserId != userId)
        {
            return ServiceResult<CategoryResponseDto>.NotFound("Category not found.");
        }

        _mapper.Map(request, category);
        category.UpdatedAtUtc = DateTime.UtcNow;

        _unitOfWork.Categories.Update(category);
        await _unitOfWork.SaveChangesAsync();

        var response = _mapper.Map<CategoryResponseDto>(category);
        return ServiceResult<CategoryResponseDto>.Success(response, "Category updated successfully.");
    }

    public async Task<ServiceResult<bool>> DeleteCategoryAsync(Guid categoryId, string userId)
    {
        var category = await _unitOfWork.Categories.GetCategoryWithWalletAsync(categoryId);
        if (category == null || category.Wallet.UserId != userId)
        {
            return ServiceResult<bool>.NotFound("Category not found.");
        }

        _unitOfWork.Categories.Remove(category);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult<bool>.Success(true, "Category deleted successfully.");
    }
}
