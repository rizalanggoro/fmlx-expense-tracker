using ExpenseTracker.Api.Domain.Entities;

namespace ExpenseTracker.Api.Repositories.Interfaces;

public interface IWalletRepository : IGenericRepository<Wallet>
{
    Task<IReadOnlyList<Wallet>> GetWalletsByUserIdAsync(string userId);
    Task<Wallet?> GetWalletWithDetailsAsync(Guid walletId, string userId);
    Task<bool> ExistsAsync(Guid walletId, string userId);
}

