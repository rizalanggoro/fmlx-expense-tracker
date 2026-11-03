using ExpenseTracker.Api.Common;
using ExpenseTracker.Api.Domain.Entities;

namespace ExpenseTracker.Api.Repositories.Interfaces;

public interface ITransactionRepository
{
    Task<Transaction> CreateAsync(Transaction transaction);

    Task<PagedResult<Transaction>> GetAllPagedAsync(
        string userId,
        Guid? categoryId = null,
        int pageIndex = 1,
        int pageSize = 20,
        DateTime? startDate = null,
        DateTime? endDate = null
    );

    Task<Transaction?> GetByIdAsync(string userId, Guid transactionId);
    Task UpdateAsync(Transaction transaction);
    Task DeleteAsync(Transaction transaction);
}
