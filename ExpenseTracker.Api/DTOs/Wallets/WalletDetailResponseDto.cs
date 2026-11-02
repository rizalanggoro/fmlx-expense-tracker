using ExpenseTracker.Api.DTOs.Categories;

namespace ExpenseTracker.Api.DTOs.Wallets;

public class WalletDetailResponseDto : WalletResponseDto
{
    public IReadOnlyList<CategoryResponseDto> Categories { get; set; } = [];
}
