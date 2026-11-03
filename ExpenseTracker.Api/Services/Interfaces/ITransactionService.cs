using ExpenseTracker.Api.Common;
using ExpenseTracker.Api.DTOs.Transactions;

namespace ExpenseTracker.Api.Services.Interfaces;

public interface ITransactionService
{
    Task<ServiceResult<TransactionResponseDto>> CreateTransactionAsync(
        string userId,
        CreateTransactionDto request
    );

    Task<ServiceResult<PagedResult<TransactionResponseDto>>> GetPagedTransactionsAsync(
        string userId,
        Guid? categoryId = null,
        int pageIndex = 1,
        int pageSize = 20,
        DateTime? startDate = null,
        DateTime? endDate = null
    );

    Task<ServiceResult<TransactionResponseDto>> GetTransactionByIdAsync(
        string userId,
        Guid transactionId
    );

    Task<ServiceResult<TransactionResponseDto>> UpdateTransactionAsync(
        string userId,
        Guid transactionId,
        UpdateTransactionDto request
    );

    Task<ServiceResult<bool>> DeleteTransactionAsync(
        string userId,
        Guid transactionId
    );
}
