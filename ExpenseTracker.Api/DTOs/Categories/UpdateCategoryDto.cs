using ExpenseTracker.Api.Domain.Enums;

namespace ExpenseTracker.Api.DTOs.Categories;

public record UpdateCategoryDto
{
    public string Name { get; init; } = string.Empty;
    public CategoryType Type { get; init; }
}