using ExpenseTracker.Api.Domain.Enums;

namespace ExpenseTracker.Api.DTOs.Categories;

public class CreateCategoryDto
{
    public string Name { get; set; } = string.Empty;
    public CategoryType Type { get; set; }
}
