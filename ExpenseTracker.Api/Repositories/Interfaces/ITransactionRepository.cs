using ExpenseTracker.Api.Common;
using ExpenseTracker.Api.Domain.Entities;

namespace ExpenseTracker.Api.Repositories.Interfaces;

public interface ITransactionRepository
{
    Task<Transaction> CreateTransactionAsync(Transaction transaction);

    Task<PagedResult<Transaction>> GetPagedTransactionsAsync(
        string userId,
        Guid? categoryId = null,
        int pageIndex = 1,
        int pageSize = 20,
        DateTime? startDate = null,
        DateTime? endDate = null
    );

    Task<Transaction?> GetTransactionAsync(string userId, Guid transactionId);
    Task UpdateTransactionAsync(Transaction transaction);
    Task DeleteTransactionAsync(Transaction transaction);
}