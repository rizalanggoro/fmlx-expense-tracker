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
    public async Task<ServiceResult<WalletResponseDto>> CreateWalletAsync(
        string userId,
        CreateWalletDto request
    )
    {
        var wallet = mapper.Map<Wallet>(request);
        wallet.Id = Guid.NewGuid();
        wallet.UserId = userId;
        wallet.CreatedAtUtc = DateTime.UtcNow;

        await walletRepository.CreateWalletAsync(wallet);
        var response = mapper.Map<WalletResponseDto>(wallet);

        return ServiceResult<WalletResponseDto>.Success(
            response,
            "Wallet created successfully.",
            201
        );
    }

    public async Task<ServiceResult<IReadOnlyList<WalletResponseDto>>> GetWalletsAsync(
        string userId
    )
    {
        var wallets = await walletRepository.GetWalletsAsync(userId);
        var response = mapper.Map<IReadOnlyList<WalletResponseDto>>(wallets);

        return ServiceResult<IReadOnlyList<WalletResponseDto>>.Success(response);
    }

    public async Task<ServiceResult<WalletDetailResponseDto>> GetWalletAsync(
        string userId,
        Guid walletId
    )
    {
        var wallet = await walletRepository.GetWalletWithDetailsAsync(userId, walletId);
        if (wallet == null)
            return ServiceResult<WalletDetailResponseDto>.Failure(
                "Wallet not found.",
                404
            );

        var response = mapper.Map<WalletDetailResponseDto>(wallet);

        return ServiceResult<WalletDetailResponseDto>.Success(response);
    }

    public async Task<ServiceResult<WalletResponseDto>> UpdateWalletAsync(
        string userId,
        Guid walletId,
        UpdateWalletDto request
    )
    {
        var wallet = await walletRepository.GetWalletAsync(userId, walletId);
        if (wallet == null)
            return ServiceResult<WalletResponseDto>.Failure(
                "Wallet not found.",
                404
            );

        mapper.Map(request, wallet);
        wallet.UpdatedAtUtc = DateTime.UtcNow;

        await walletRepository.UpdateWalletAsync(wallet);
        var response = mapper.Map<WalletResponseDto>(wallet);

        return ServiceResult<WalletResponseDto>.Success(
            response,
            "Wallet updated successfully."
        );
    }

    public async Task<ServiceResult<bool>> DeleteWalletAsync(string userId, Guid walletId)
    {
        var wallet = await walletRepository.GetWalletAsync(userId, walletId);
        if (wallet == null)
            return ServiceResult<bool>.Failure(
                "Wallet not found.",
                404
            );

        await walletRepository.DeleteWalletAsync(wallet);

        return ServiceResult<bool>.Success(
            true,
            "Wallet deleted successfully."
        );
    }
}