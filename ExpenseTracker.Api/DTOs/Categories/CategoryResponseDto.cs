using ExpenseTracker.Api.Domain.Enums;

namespace ExpenseTracker.Api.DTOs.Categories;

public class CategoryResponseDto
{
    public Guid Id { get; set; }
    public Guid WalletId { get; set; }
    public string Name { get; set; } = string.Empty;
    public CategoryType Type { get; set; }
    public string TypeName => Type.ToString();
    public decimal TotalAmount { get; set; }
    public int TransactionCount { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
}
