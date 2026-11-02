using ExpenseTracker.Api.Common;
using ExpenseTracker.Api.DTOs.Wallets;

namespace ExpenseTracker.Api.Services.Interfaces;

public interface IWalletService
{
    Task<ServiceResult<IReadOnlyList<WalletResponseDto>>> GetWalletsAsync(string userId);
    Task<ServiceResult<WalletDetailResponseDto>> GetWalletByIdAsync(Guid walletId, string userId);
    Task<ServiceResult<WalletResponseDto>> CreateWalletAsync(string userId, CreateWalletDto request);
    Task<ServiceResult<WalletResponseDto>> UpdateWalletAsync(Guid walletId, string userId, UpdateWalletDto request);
    Task<ServiceResult<bool>> DeleteWalletAsync(Guid walletId, string userId);
}

