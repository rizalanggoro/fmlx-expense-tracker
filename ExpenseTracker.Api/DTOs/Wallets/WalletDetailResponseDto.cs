using ExpenseTracker.Api.DTOs.Categories;

namespace ExpenseTracker.Api.DTOs.Wallets;

public record WalletDetailResponseDto : WalletResponseDto
{
    public IReadOnlyList<CategoryResponseDto> Categories { get; init; } = [];
}
