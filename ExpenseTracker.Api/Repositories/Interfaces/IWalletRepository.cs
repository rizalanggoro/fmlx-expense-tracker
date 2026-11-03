using ExpenseTracker.Api.Domain.Entities;

namespace ExpenseTracker.Api.Repositories.Interfaces;

public interface IWalletRepository
{
    Task<Wallet> CreateWalletAsync(Wallet wallet);
    Task<IReadOnlyList<Wallet>> GetWalletsAsync(string userId);
    Task<Wallet?> GetWalletAsync(string userId, Guid walletId);
    Task<Wallet?> GetWalletWithDetailsAsync(string userId, Guid walletId);
    Task UpdateWalletAsync(Wallet wallet);
    Task DeleteWalletAsync(Wallet wallet);
}