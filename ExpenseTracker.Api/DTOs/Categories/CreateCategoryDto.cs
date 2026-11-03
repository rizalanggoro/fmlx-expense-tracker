using ExpenseTracker.Api.Domain.Enums;

namespace ExpenseTracker.Api.DTOs.Categories;

public record CreateCategoryDto(
    Guid WalletId,
    CategoryType Type
)
{
    public string Name => string.Empty;
}