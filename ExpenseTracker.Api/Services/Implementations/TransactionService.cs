using AutoMapper;
using ExpenseTracker.Api.Common;
using ExpenseTracker.Api.Domain.Entities;
using ExpenseTracker.Api.DTOs.Transactions;
using ExpenseTracker.Api.Repositories.Interfaces;
using ExpenseTracker.Api.Services.Interfaces;

namespace ExpenseTracker.Api.Services.Implementations;

public class TransactionService(
    ITransactionRepository transactionRepository,
    ICategoryRepository categoryRepository,
    IMapper mapper
) : ITransactionService
{
    public async Task<ServiceResult<IReadOnlyList<TransactionResponseDto>>> GetTransactionsByCategoryIdAsync(Guid categoryId, string userId)
    {
        var categoryBelongs = await categoryRepository.CategoryBelongsToUserAsync(categoryId, userId);
        if (!categoryBelongs)
        {
            return ServiceResult<IReadOnlyList<TransactionResponseDto>>.NotFound("Category not found.");
        }

        var transactions = await transactionRepository.GetTransactionsByCategoryIdAsync(categoryId);
        var response = mapper.Map<IReadOnlyList<TransactionResponseDto>>(transactions);
        return ServiceResult<IReadOnlyList<TransactionResponseDto>>.Success(response);
    }

    public async Task<ServiceResult<PagedResult<TransactionResponseDto>>> GetPagedTransactionsByCategoryIdAsync(
        Guid categoryId, string userId, int pageIndex, int pageSize, DateTime? startDate = null, DateTime? endDate = null)
    {
        var categoryBelongs = await categoryRepository.CategoryBelongsToUserAsync(categoryId, userId);
        if (!categoryBelongs)
        {
            return ServiceResult<PagedResult<TransactionResponseDto>>.NotFound("Category not found.");
        }

        var (items, totalCount) = await transactionRepository.GetPagedTransactionsByCategoryIdAsync(
            categoryId, pageIndex, pageSize, startDate, endDate);

        var mappedItems = mapper.Map<IReadOnlyList<TransactionResponseDto>>(items);
        var pagedResult = new PagedResult<TransactionResponseDto>(mappedItems, totalCount, pageIndex, pageSize);

        return ServiceResult<PagedResult<TransactionResponseDto>>.Success(pagedResult);
    }

    public async Task<ServiceResult<TransactionResponseDto>> GetTransactionByIdAsync(Guid transactionId, string userId)
    {
        var transaction = await transactionRepository.GetTransactionWithCategoryAndWalletAsync(transactionId);
        if (transaction == null || transaction.Category.Wallet.UserId != userId)
        {
            return ServiceResult<TransactionResponseDto>.NotFound("Transaction not found.");
        }

        var response = mapper.Map<TransactionResponseDto>(transaction);
        return ServiceResult<TransactionResponseDto>.Success(response);
    }

    public async Task<ServiceResult<TransactionResponseDto>> CreateTransactionAsync(Guid categoryId, string userId, CreateTransactionDto request)
    {
        var category = await categoryRepository.GetCategoryWithWalletAsync(categoryId);
        if (category == null || category.Wallet.UserId != userId)
        {
            return ServiceResult<TransactionResponseDto>.NotFound("Category not found.");
        }

        var transaction = mapper.Map<Transaction>(request);
        transaction.Id = Guid.NewGuid();
        transaction.CategoryId = categoryId;
        transaction.CreatedAtUtc = DateTime.UtcNow;

        await transactionRepository.AddAsync(transaction);

        // Assign navigation category for mapper response
        transaction.Category = category;

        var response = mapper.Map<TransactionResponseDto>(transaction);
        return ServiceResult<TransactionResponseDto>.Success(response, "Transaction created successfully.", statusCode: 201);
    }

    public async Task<ServiceResult<TransactionResponseDto>> UpdateTransactionAsync(Guid transactionId, string userId, UpdateTransactionDto request)
    {
        var transaction = await transactionRepository.GetTransactionWithCategoryAndWalletAsync(transactionId);
        if (transaction == null || transaction.Category.Wallet.UserId != userId)
        {
            return ServiceResult<TransactionResponseDto>.NotFound("Transaction not found.");
        }

        mapper.Map(request, transaction);
        transaction.UpdatedAtUtc = DateTime.UtcNow;

        await transactionRepository.UpdateAsync(transaction);

        var response = mapper.Map<TransactionResponseDto>(transaction);
        return ServiceResult<TransactionResponseDto>.Success(response, "Transaction updated successfully.");
    }

    public async Task<ServiceResult<bool>> DeleteTransactionAsync(Guid transactionId, string userId)
    {
        var transaction = await transactionRepository.GetTransactionWithCategoryAndWalletAsync(transactionId);
        if (transaction == null || transaction.Category.Wallet.UserId != userId)
        {
            return ServiceResult<bool>.NotFound("Transaction not found.");
        }

        await transactionRepository.DeleteAsync(transaction);

        return ServiceResult<bool>.Success(true, "Transaction deleted successfully.");
    }
}
