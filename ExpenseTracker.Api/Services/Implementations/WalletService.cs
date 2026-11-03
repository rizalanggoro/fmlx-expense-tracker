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
    public async Task<ServiceResult<WalletResponseDto>> CreateAsync(
        string userId,
        CreateWalletDto request
    )
    {
        var wallet = mapper.Map<Wallet>(request);
        wallet.Id = Guid.NewGuid();
        wallet.UserId = userId;
        wallet.CreatedAtUtc = DateTime.UtcNow;

        await walletRepository.CreateAsync(wallet);
        var response = mapper.Map<WalletResponseDto>(wallet);

        return ServiceResult<WalletResponseDto>.Success(
            response,
            "Wallet created successfully.",
            201
        );
    }

    public async Task<ServiceResult<IReadOnlyList<WalletResponseDto>>> GetAllAsync(
        string userId
    )
    {
        var wallets = await walletRepository.GetAllAsync(userId);
        var response = mapper.Map<IReadOnlyList<WalletResponseDto>>(wallets);

        return ServiceResult<IReadOnlyList<WalletResponseDto>>.Success(response);
    }

    public async Task<ServiceResult<WalletDetailResponseDto>> GetAsync(
        string userId,
        Guid walletId
    )
    {
        var wallet = await walletRepository.GetWithDetailsAsync(userId, walletId);
        if (wallet == null)
            return ServiceResult<WalletDetailResponseDto>.Failure(
                "Wallet not found.",
                404
            );

        var response = mapper.Map<WalletDetailResponseDto>(wallet);

        return ServiceResult<WalletDetailResponseDto>.Success(response);
    }

    public async Task<ServiceResult<WalletResponseDto>> UpdateAsync(
        string userId,
        Guid walletId,
        UpdateWalletDto request
    )
    {
        var wallet = await walletRepository.GetAsync(userId, walletId);
        if (wallet == null)
            return ServiceResult<WalletResponseDto>.Failure(
                "Wallet not found.",
                404
            );

        mapper.Map(request, wallet);
        wallet.UpdatedAtUtc = DateTime.UtcNow;

        await walletRepository.UpdateAsync(wallet);
        var response = mapper.Map<WalletResponseDto>(wallet);

        return ServiceResult<WalletResponseDto>.Success(
            response,
            "Wallet updated successfully."
        );
    }

    public async Task<ServiceResult<bool>> DeleteAsync(string userId, Guid walletId)
    {
        var wallet = await walletRepository.GetAsync(userId, walletId);
        if (wallet == null)
            return ServiceResult<bool>.Failure(
                "Wallet not found.",
                404
            );

        await walletRepository.DeleteAsync(wallet);

        return ServiceResult<bool>.Success(
            true,
            "Wallet deleted successfully."
        );
    }
}