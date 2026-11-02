namespace ExpenseTracker.Api.DTOs.Transactions;

public class UpdateTransactionDto
{
    public decimal Amount { get; set; }
    public DateTime TransactionDate { get; set; }
    public string? Description { get; set; }
}
