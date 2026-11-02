using AutoMapper;
using ExpenseTracker.Api.Common;
using ExpenseTracker.Api.Domain.Entities;
using ExpenseTracker.Api.DTOs.Wallets;
using ExpenseTracker.Api.Repositories.Interfaces;
using ExpenseTracker.Api.Services.Interfaces;

namespace ExpenseTracker.Api.Services.Implementations;

public class WalletService : IWalletService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public WalletService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ServiceResult<IReadOnlyList<WalletResponseDto>>> GetWalletsAsync(string userId)
    {
        var wallets = await _unitOfWork.Wallets.GetWalletsByUserIdAsync(userId);
        var response = _mapper.Map<IReadOnlyList<WalletResponseDto>>(wallets);
        return ServiceResult<IReadOnlyList<WalletResponseDto>>.Success(response);
    }

    public async Task<ServiceResult<WalletDetailResponseDto>> GetWalletByIdAsync(Guid walletId, string userId)
    {
        var wallet = await _unitOfWork.Wallets.GetWalletWithDetailsAsync(walletId, userId);
        if (wallet == null)
        {
            return ServiceResult<WalletDetailResponseDto>.NotFound("Wallet not found.");
        }

        var response = _mapper.Map<WalletDetailResponseDto>(wallet);
        return ServiceResult<WalletDetailResponseDto>.Success(response);
    }

    public async Task<ServiceResult<WalletResponseDto>> CreateWalletAsync(string userId, CreateWalletDto request)
    {
        var wallet = _mapper.Map<Wallet>(request);
        wallet.Id = Guid.NewGuid();
        wallet.UserId = userId;
        wallet.CreatedAtUtc = DateTime.UtcNow;

        await _unitOfWork.Wallets.AddAsync(wallet);
        await _unitOfWork.SaveChangesAsync();

        var response = _mapper.Map<WalletResponseDto>(wallet);
        return ServiceResult<WalletResponseDto>.Success(response, "Wallet created successfully.", statusCode: 201);
    }

    public async Task<ServiceResult<WalletResponseDto>> UpdateWalletAsync(Guid walletId, string userId, UpdateWalletDto request)
    {
        var wallet = await _unitOfWork.Wallets.GetWalletWithDetailsAsync(walletId, userId);
        if (wallet == null)
        {
            return ServiceResult<WalletResponseDto>.NotFound("Wallet not found.");
        }

        _mapper.Map(request, wallet);
        wallet.UpdatedAtUtc = DateTime.UtcNow;

        _unitOfWork.Wallets.Update(wallet);
        await _unitOfWork.SaveChangesAsync();

        var response = _mapper.Map<WalletResponseDto>(wallet);
        return ServiceResult<WalletResponseDto>.Success(response, "Wallet updated successfully.");
    }

    public async Task<ServiceResult<bool>> DeleteWalletAsync(Guid walletId, string userId)
    {
        var wallet = await _unitOfWork.Wallets.GetWalletWithDetailsAsync(walletId, userId);
        if (wallet == null)
        {
            return ServiceResult<bool>.NotFound("Wallet not found.");
        }

        _unitOfWork.Wallets.Remove(wallet);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult<bool>.Success(true, "Wallet deleted successfully.");
    }
}
