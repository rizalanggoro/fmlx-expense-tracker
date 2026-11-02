using ExpenseTracker.Api.Data;
using ExpenseTracker.Api.Domain.Entities;
using ExpenseTracker.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Api.Repositories.Implementations;

public class TransactionRepository : GenericRepository<Transaction>, ITransactionRepository
{
    public TransactionRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Transaction>> GetTransactionsByCategoryIdAsync(Guid categoryId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(t => t.CategoryId == categoryId)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync();
    }

    public async Task<(IReadOnlyList<Transaction> Items, int TotalCount)> GetPagedTransactionsByCategoryIdAsync(
        Guid categoryId, int pageIndex, int pageSize, DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _dbSet
            .AsNoTracking()
            .Where(t => t.CategoryId == categoryId);

        if (startDate.HasValue)
        {
            query = query.Where(t => t.TransactionDate >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(t => t.TransactionDate <= endDate.Value);
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(t => t.TransactionDate)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<Transaction?> GetTransactionWithCategoryAndWalletAsync(Guid transactionId)
    {
        return await _dbSet
            .Include(t => t.Category)
                .ThenInclude(c => c.Wallet)
            .FirstOrDefaultAsync(t => t.Id == transactionId);
    }

    public async Task<bool> TransactionBelongsToUserAsync(Guid transactionId, string userId)
    {
        return await _dbSet
            .AsNoTracking()
            .AnyAsync(t => t.Id == transactionId && t.Category.Wallet.UserId == userId);
    }
}
