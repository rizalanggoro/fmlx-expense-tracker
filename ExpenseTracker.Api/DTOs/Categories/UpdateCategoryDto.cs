using ExpenseTracker.Api.Domain.Enums;

namespace ExpenseTracker.Api.DTOs.Categories;

public record UpdateCategoryDto(
    CategoryType Type
)
{
    public string Name => string.Empty;
}