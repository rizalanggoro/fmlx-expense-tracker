using ExpenseTracker.Api.Data;
using ExpenseTracker.Api.Domain.Entities;
using ExpenseTracker.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Api.Repositories.Implementations;

public class TransactionRepository(ApplicationDbContext context) : ITransactionRepository
{
    public async Task<(IReadOnlyList<Transaction> Items, int TotalCount)> GetPagedTransactionsAsync(
        string userId,
        Guid? categoryId = null,
        int pageIndex = 1,
        int pageSize = 20,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        var query = context.Transactions
            .AsNoTracking()
            .Include(t => t.Category)
            .Where(t => t.Category.Wallet.UserId == userId);

        if (categoryId.HasValue)
        {
            query = query.Where(t => t.CategoryId == categoryId.Value);
        }

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

    public async Task<IReadOnlyList<Transaction>> GetTransactionsByCategoryIdAsync(Guid categoryId)
    {
        return await context.Transactions
            .AsNoTracking()
            .Where(t => t.CategoryId == categoryId)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync();
    }

    public async Task<(IReadOnlyList<Transaction> Items, int TotalCount)> GetPagedTransactionsByCategoryIdAsync(
        Guid categoryId, int pageIndex, int pageSize, DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = context.Transactions
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
        return await context.Transactions
            .Include(t => t.Category)
                .ThenInclude(c => c.Wallet)
            .FirstOrDefaultAsync(t => t.Id == transactionId);
    }

    public async Task<bool> TransactionBelongsToUserAsync(Guid transactionId, string userId)
    {
        return await context.Transactions
            .AsNoTracking()
            .AnyAsync(t => t.Id == transactionId && t.Category.Wallet.UserId == userId);
    }

    public async Task<Transaction> AddAsync(Transaction transaction)
    {
        await context.Transactions.AddAsync(transaction);
        await context.SaveChangesAsync();
        return transaction;
    }

    public async Task UpdateAsync(Transaction transaction)
    {
        context.Transactions.Update(transaction);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Transaction transaction)
    {
        context.Transactions.Remove(transaction);
        await context.SaveChangesAsync();
    }
}
