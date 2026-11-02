using AutoMapper;
using ExpenseTracker.Api.Common;
using ExpenseTracker.Api.Domain.Entities;
using ExpenseTracker.Api.DTOs.Wallets;
using ExpenseTracker.Api.Repositories.Interfaces;
using ExpenseTracker.Api.Services.Interfaces;

namespace ExpenseTracker.Api.Services.Implementations;

public class WalletService(
    IWalletRepository walletRepository,
    IMapper mapper
) : IWalletService
{
    public async Task<ServiceResult<IReadOnlyList<WalletResponseDto>>> GetWalletsAsync(string userId)
    {
        var wallets = await walletRepository.GetWalletsByUserIdAsync(userId);
        var response = mapper.Map<IReadOnlyList<WalletResponseDto>>(wallets);

        return ServiceResult<IReadOnlyList<WalletResponseDto>>.Success(response);
    }

    public async Task<ServiceResult<WalletDetailResponseDto>> GetWalletByIdAsync(Guid walletId, string userId)
    {
        var wallet = await walletRepository.GetWalletWithDetailsAsync(walletId, userId);
        if (wallet == null)
            return ServiceResult<WalletDetailResponseDto>.NotFound("Wallet not found.");

        var response = mapper.Map<WalletDetailResponseDto>(wallet);

        return ServiceResult<WalletDetailResponseDto>.Success(response);
    }

    public async Task<ServiceResult<WalletResponseDto>> CreateWalletAsync(string userId, CreateWalletDto request)
    {
        var wallet = mapper.Map<Wallet>(request);
        wallet.Id = Guid.NewGuid();
        wallet.UserId = userId;
        wallet.CreatedAtUtc = DateTime.UtcNow;

        await walletRepository.AddAsync(wallet);

        var response = mapper.Map<WalletResponseDto>(wallet);

        return ServiceResult<WalletResponseDto>.Success(
            response,
            "Wallet created successfully.",
            201
        );
    }

    public async Task<ServiceResult<WalletResponseDto>> UpdateWalletAsync(Guid walletId, string userId,
        UpdateWalletDto request)
    {
        var wallet = await walletRepository.GetWalletWithDetailsAsync(walletId, userId);
        if (wallet == null)
            return ServiceResult<WalletResponseDto>.NotFound("Wallet not found.");

        mapper.Map(request, wallet);
        wallet.UpdatedAtUtc = DateTime.UtcNow;

        await walletRepository.UpdateAsync(wallet);

        var response = mapper.Map<WalletResponseDto>(wallet);

        return ServiceResult<WalletResponseDto>.Success(
            response,
            "Wallet updated successfully."
        );
    }

    public async Task<ServiceResult<bool>> DeleteWalletAsync(Guid walletId, string userId)
    {
        var wallet = await walletRepository.GetWalletWithDetailsAsync(walletId, userId);
        if (wallet == null)
            return ServiceResult<bool>.NotFound("Wallet not found.");

        await walletRepository.DeleteAsync(wallet);

        return ServiceResult<bool>.Success(
            true,
            "Wallet deleted successfully."
        );
    }
}