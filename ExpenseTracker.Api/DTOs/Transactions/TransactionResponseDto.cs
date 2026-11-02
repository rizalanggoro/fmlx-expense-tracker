using ExpenseTracker.Api.Domain.Enums;

namespace ExpenseTracker.Api.DTOs.Transactions;

public class TransactionResponseDto
{
    public Guid Id { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public CategoryType CategoryType { get; set; }
    public string CategoryTypeName => CategoryType.ToString();
    public decimal Amount { get; set; }
    public DateTime TransactionDate { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
}

