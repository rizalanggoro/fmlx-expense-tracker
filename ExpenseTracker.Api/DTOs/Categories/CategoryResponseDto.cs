using ExpenseTracker.Api.Domain.Enums;

namespace ExpenseTracker.Api.DTOs.Categories;

public record CategoryResponseDto
{
    public Guid Id { get; init; }
    public Guid WalletId { get; init; }
    public string Name { get; init; } = string.Empty;
    public CategoryType Type { get; init; }
    public string TypeName => Type.ToString();
    public decimal TotalAmount { get; init; }
    public int TransactionCount { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}
