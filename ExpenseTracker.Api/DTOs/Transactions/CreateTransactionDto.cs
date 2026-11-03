namespace ExpenseTracker.Api.DTOs.Transactions;

public record CreateTransactionDto
{
    public Guid CategoryId { get; init; }
    public decimal Amount { get; init; }
    public DateTime TransactionDate { get; init; } = DateTime.UtcNow;
    public string? Description { get; init; }
}