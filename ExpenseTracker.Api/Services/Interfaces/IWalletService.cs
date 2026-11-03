using ExpenseTracker.Api.Common;
using ExpenseTracker.Api.DTOs.Wallets;

namespace ExpenseTracker.Api.Services.Interfaces;

public interface IWalletService
{
    Task<ServiceResult<WalletResponseDto>> CreateWalletAsync(
        string userId,
        CreateWalletDto request
    );

    Task<ServiceResult<IReadOnlyList<WalletResponseDto>>> GetWalletsAsync(
        string userId
    );

    Task<ServiceResult<WalletDetailResponseDto>> GetWalletAsync(
        string userId,
        Guid walletId
    );

    Task<ServiceResult<WalletResponseDto>> UpdateWalletAsync(
        string userId,
        Guid walletId,
        UpdateWalletDto request
    );

    Task<ServiceResult<bool>> DeleteWalletAsync(
        string userId,
        Guid walletId
    );
}