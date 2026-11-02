namespace ExpenseTracker.Api.DTOs.Transactions;

public class CreateTransactionDto
{
    public Guid CategoryId { get; set; }
    public decimal Amount { get; set; }
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
    public string? Description { get; set; }
}
