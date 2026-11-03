using ExpenseTracker.Api.Domain.Enums;

namespace ExpenseTracker.Api.DTOs.Transactions;

public record TransactionResponseDto
{
    public Guid Id { get; init; }
    public Guid CategoryId { get; init; }
    public string CategoryName { get; init; } = string.Empty;
    public CategoryType CategoryType { get; init; }
    public string CategoryTypeName => CategoryType.ToString();
    public decimal Amount { get; init; }
    public DateTime TransactionDate { get; init; }
    public string? Description { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}
