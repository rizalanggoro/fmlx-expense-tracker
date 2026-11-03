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
    public async Task<ServiceResult<TransactionResponseDto>> CreateTransactionAsync(
        string userId,
        CreateTransactionDto request
    )
    {
        var category = await categoryRepository.GetCategoryAsync(userId, request.CategoryId);
        if (category == null)
            return ServiceResult<TransactionResponseDto>.Failure(
                "Category not found.",
                404
            );

        var transaction = mapper.Map<Transaction>(request);
        transaction.Id = Guid.NewGuid();
        transaction.CreatedAtUtc = DateTime.UtcNow;

        await transactionRepository.CreateTransactionAsync(transaction);

        // Assign navigation category for mapper response
        transaction.Category = category;

        var response = mapper.Map<TransactionResponseDto>(transaction);

        return ServiceResult<TransactionResponseDto>.Success(
            response,
            "Transaction created successfully.",
            201
        );
    }

    public async Task<ServiceResult<PagedResult<TransactionResponseDto>>> GetPagedTransactionsAsync(
        string userId,
        Guid? categoryId = null,
        int pageIndex = 1,
        int pageSize = 20,
        DateTime? startDate = null,
        DateTime? endDate = null
    )
    {
        if (categoryId.HasValue)
        {
            var category = await categoryRepository.GetCategoryAsync(userId, categoryId.Value);
            if (category == null)
                return ServiceResult<PagedResult<TransactionResponseDto>>.Failure(
                    "Category not found.",
                    404
                );
        }

        var pagedTransactions = await transactionRepository.GetPagedTransactionsAsync(
            userId,
            categoryId,
            pageIndex,
            pageSize,
            startDate,
            endDate
        );

        var mappedItems = mapper.Map<IReadOnlyList<TransactionResponseDto>>(pagedTransactions.Items);
        var pagedResult = new PagedResult<TransactionResponseDto>(
            mappedItems,
            pagedTransactions.TotalCount,
            pagedTransactions.PageIndex,
            pagedTransactions.PageSize
        );

        return ServiceResult<PagedResult<TransactionResponseDto>>.Success(pagedResult);
    }

    public async Task<ServiceResult<TransactionResponseDto>> GetTransactionAsync(
        string userId,
        Guid transactionId
    )
    {
        var transaction = await transactionRepository.GetTransactionAsync(userId, transactionId);
        if (transaction == null)
            return ServiceResult<TransactionResponseDto>.Failure(
                "Transaction not found.",
                404
            );

        var response = mapper.Map<TransactionResponseDto>(transaction);

        return ServiceResult<TransactionResponseDto>.Success(response);
    }

    public async Task<ServiceResult<TransactionResponseDto>> UpdateTransactionAsync(
        string userId,
        Guid transactionId,
        UpdateTransactionDto request
    )
    {
        var transaction = await transactionRepository.GetTransactionAsync(userId, transactionId);
        if (transaction == null)
            return ServiceResult<TransactionResponseDto>.Failure(
                "Transaction not found.",
                404
            );

        mapper.Map(request, transaction);
        transaction.UpdatedAtUtc = DateTime.UtcNow;

        await transactionRepository.UpdateTransactionAsync(transaction);

        var response = mapper.Map<TransactionResponseDto>(transaction);

        return ServiceResult<TransactionResponseDto>.Success(
            response,
            "Transaction updated successfully."
        );
    }

    public async Task<ServiceResult<bool>> DeleteTransactionAsync(string userId, Guid transactionId)
    {
        var transaction = await transactionRepository.GetTransactionAsync(userId, transactionId);
        if (transaction == null)
            return ServiceResult<bool>.Failure(
                "Transaction not found.",
                404
            );

        await transactionRepository.DeleteTransactionAsync(transaction);

        return ServiceResult<bool>.Success(
            true,
            "Transaction deleted successfully."
        );
    }
}