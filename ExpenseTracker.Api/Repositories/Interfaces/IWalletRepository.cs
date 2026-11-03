using ExpenseTracker.Api.Domain.Entities;

namespace ExpenseTracker.Api.Repositories.Interfaces;

public interface IWalletRepository
{
    Task<Wallet> CreateAsync(Wallet wallet);
    Task<IReadOnlyList<Wallet>> GetAllAsync(string userId);
    Task<Wallet?> GetAsync(string userId, Guid walletId);
    Task<Wallet?> GetWithDetailsAsync(string userId, Guid walletId);
    Task UpdateAsync(Wallet wallet);
    Task DeleteAsync(Wallet wallet);
}