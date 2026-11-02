using AutoMapper;
using ExpenseTracker.Api.Common;
using ExpenseTracker.Api.Domain.Entities;
using ExpenseTracker.Api.DTOs.Transactions;
using ExpenseTracker.Api.Repositories.Interfaces;
using ExpenseTracker.Api.Services.Interfaces;

namespace ExpenseTracker.Api.Services.Implementations;

public class TransactionService : ITransactionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public TransactionService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ServiceResult<IReadOnlyList<TransactionResponseDto>>> GetTransactionsByCategoryIdAsync(Guid categoryId, string userId)
    {
        var categoryBelongs = await _unitOfWork.Categories.CategoryBelongsToUserAsync(categoryId, userId);
        if (!categoryBelongs)
        {
            return ServiceResult<IReadOnlyList<TransactionResponseDto>>.NotFound("Category not found.");
        }

        var transactions = await _unitOfWork.Transactions.GetTransactionsByCategoryIdAsync(categoryId);
        var response = _mapper.Map<IReadOnlyList<TransactionResponseDto>>(transactions);
        return ServiceResult<IReadOnlyList<TransactionResponseDto>>.Success(response);
    }

    public async Task<ServiceResult<PagedResult<TransactionResponseDto>>> GetPagedTransactionsByCategoryIdAsync(
        Guid categoryId, string userId, int pageIndex, int pageSize, DateTime? startDate = null, DateTime? endDate = null)
    {
        var categoryBelongs = await _unitOfWork.Categories.CategoryBelongsToUserAsync(categoryId, userId);
        if (!categoryBelongs)
        {
            return ServiceResult<PagedResult<TransactionResponseDto>>.NotFound("Category not found.");
        }

        var (items, totalCount) = await _unitOfWork.Transactions.GetPagedTransactionsByCategoryIdAsync(
            categoryId, pageIndex, pageSize, startDate, endDate);

        var mappedItems = _mapper.Map<IReadOnlyList<TransactionResponseDto>>(items);
        var pagedResult = new PagedResult<TransactionResponseDto>(mappedItems, totalCount, pageIndex, pageSize);

        return ServiceResult<PagedResult<TransactionResponseDto>>.Success(pagedResult);
    }

    public async Task<ServiceResult<TransactionResponseDto>> GetTransactionByIdAsync(Guid transactionId, string userId)
    {
        var transaction = await _unitOfWork.Transactions.GetTransactionWithCategoryAndWalletAsync(transactionId);
        if (transaction == null || transaction.Category.Wallet.UserId != userId)
        {
            return ServiceResult<TransactionResponseDto>.NotFound("Transaction not found.");
        }

        var response = _mapper.Map<TransactionResponseDto>(transaction);
        return ServiceResult<TransactionResponseDto>.Success(response);
    }

    public async Task<ServiceResult<TransactionResponseDto>> CreateTransactionAsync(Guid categoryId, string userId, CreateTransactionDto request)
    {
        var category = await _unitOfWork.Categories.GetCategoryWithWalletAsync(categoryId);
        if (category == null || category.Wallet.UserId != userId)
        {
            return ServiceResult<TransactionResponseDto>.NotFound("Category not found.");
        }

        var transaction = _mapper.Map<Transaction>(request);
        transaction.Id = Guid.NewGuid();
        transaction.CategoryId = categoryId;
        transaction.CreatedAtUtc = DateTime.UtcNow;

        await _unitOfWork.Transactions.AddAsync(transaction);
        await _unitOfWork.SaveChangesAsync();

        // Assign navigation category for mapper response
        transaction.Category = category;

        var response = _mapper.Map<TransactionResponseDto>(transaction);
        return ServiceResult<TransactionResponseDto>.Success(response, "Transaction created successfully.", statusCode: 201);
    }

    public async Task<ServiceResult<TransactionResponseDto>> UpdateTransactionAsync(Guid transactionId, string userId, UpdateTransactionDto request)
    {
        var transaction = await _unitOfWork.Transactions.GetTransactionWithCategoryAndWalletAsync(transactionId);
        if (transaction == null || transaction.Category.Wallet.UserId != userId)
        {
            return ServiceResult<TransactionResponseDto>.NotFound("Transaction not found.");
        }

        _mapper.Map(request, transaction);
        transaction.UpdatedAtUtc = DateTime.UtcNow;

        _unitOfWork.Transactions.Update(transaction);
        await _unitOfWork.SaveChangesAsync();

        var response = _mapper.Map<TransactionResponseDto>(transaction);
        return ServiceResult<TransactionResponseDto>.Success(response, "Transaction updated successfully.");
    }

    public async Task<ServiceResult<bool>> DeleteTransactionAsync(Guid transactionId, string userId)
    {
        var transaction = await _unitOfWork.Transactions.GetTransactionWithCategoryAndWalletAsync(transactionId);
        if (transaction == null || transaction.Category.Wallet.UserId != userId)
        {
            return ServiceResult<bool>.NotFound("Transaction not found.");
        }

        _unitOfWork.Transactions.Remove(transaction);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult<bool>.Success(true, "Transaction deleted successfully.");
    }
}
