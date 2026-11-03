namespace ExpenseTracker.Api.DTOs.Transactions;

public record CreateTransactionDto(
    Guid CategoryId,
    decimal Amount,
    string? Description
)
{
    public DateTime TransactionDate { get; } = DateTime.UtcNow;
}