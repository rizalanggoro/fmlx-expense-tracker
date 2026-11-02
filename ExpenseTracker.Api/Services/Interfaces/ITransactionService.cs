using ExpenseTracker.Api.Common;
using ExpenseTracker.Api.DTOs.Transactions;

namespace ExpenseTracker.Api.Services.Interfaces;

public interface ITransactionService
{
    Task<ServiceResult<IReadOnlyList<TransactionResponseDto>>> GetTransactionsByCategoryIdAsync(Guid categoryId, string userId);
    Task<ServiceResult<PagedResult<TransactionResponseDto>>> GetPagedTransactionsByCategoryIdAsync(
        Guid categoryId, string userId, int pageIndex, int pageSize, DateTime? startDate = null, DateTime? endDate = null);
    Task<ServiceResult<TransactionResponseDto>> GetTransactionByIdAsync(Guid transactionId, string userId);
    Task<ServiceResult<TransactionResponseDto>> CreateTransactionAsync(Guid categoryId, string userId, CreateTransactionDto request);
    Task<ServiceResult<TransactionResponseDto>> UpdateTransactionAsync(Guid transactionId, string userId, UpdateTransactionDto request);
    Task<ServiceResult<bool>> DeleteTransactionAsync(Guid transactionId, string userId);
}

