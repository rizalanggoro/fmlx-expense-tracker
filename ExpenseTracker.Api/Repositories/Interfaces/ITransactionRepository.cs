using ExpenseTracker.Api.Domain.Entities;

namespace ExpenseTracker.Api.Repositories.Interfaces;

public interface ITransactionRepository
{
    Task<(IReadOnlyList<Transaction> Items, int TotalCount)> GetPagedTransactionsAsync(
        string userId,
        Guid? categoryId = null,
        int pageIndex = 1,
        int pageSize = 20,
        DateTime? startDate = null,
        DateTime? endDate = null);
    Task<IReadOnlyList<Transaction>> GetTransactionsByCategoryIdAsync(Guid categoryId);
    Task<(IReadOnlyList<Transaction> Items, int TotalCount)> GetPagedTransactionsByCategoryIdAsync(
        Guid categoryId, int pageIndex, int pageSize, DateTime? startDate = null, DateTime? endDate = null);
    Task<Transaction?> GetTransactionWithCategoryAndWalletAsync(Guid transactionId);
    Task<bool> TransactionBelongsToUserAsync(Guid transactionId, string userId);
    Task<Transaction> AddAsync(Transaction transaction);
    Task UpdateAsync(Transaction transaction);
    Task DeleteAsync(Transaction transaction);
}
