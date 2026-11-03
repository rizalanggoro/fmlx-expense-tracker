using ExpenseTracker.Api.Common;
using ExpenseTracker.Api.DTOs.Wallets;

namespace ExpenseTracker.Api.Services.Interfaces;

public interface IWalletService
{
    Task<ServiceResult<WalletResponseDto>> CreateAsync(
        string userId,
        CreateWalletDto request
    );

    Task<ServiceResult<IReadOnlyList<WalletResponseDto>>> GetAllAsync(
        string userId
    );

    Task<ServiceResult<WalletDetailResponseDto>> GetAsync(
        string userId,
        Guid walletId
    );

    Task<ServiceResult<WalletResponseDto>> UpdateAsync(
        string userId,
        Guid walletId,
        UpdateWalletDto request
    );

    Task<ServiceResult<bool>> DeleteAsync(
        string userId,
        Guid walletId
    );
}