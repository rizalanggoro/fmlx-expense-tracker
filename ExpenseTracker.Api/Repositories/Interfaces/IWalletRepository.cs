using ExpenseTracker.Api.Domain.Entities;

namespace ExpenseTracker.Api.Repositories.Interfaces;

public interface IWalletRepository
{
    Task<IReadOnlyList<Wallet>> GetWalletsByUserIdAsync(string userId);
    Task<Wallet?> GetWalletWithDetailsAsync(Guid walletId, string userId);
    Task<bool> ExistsAsync(Guid walletId, string userId);
    Task<Wallet> AddAsync(Wallet wallet);
    Task UpdateAsync(Wallet wallet);
    Task DeleteAsync(Wallet wallet);
}
