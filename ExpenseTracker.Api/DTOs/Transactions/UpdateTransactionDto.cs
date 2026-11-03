namespace ExpenseTracker.Api.DTOs.Transactions;

public record UpdateTransactionDto
{
    public decimal Amount { get; init; }
    public DateTime TransactionDate { get; init; }
    public string? Description { get; init; }
}