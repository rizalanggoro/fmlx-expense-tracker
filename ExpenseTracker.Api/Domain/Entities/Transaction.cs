namespace ExpenseTracker.Api.Domain.Entities;

public class Transaction
{
    public Guid Id { get; set; }
    public Guid CategoryId { get; set; }
    public decimal Amount { get; set; }
    public DateTime TransactionDate { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }

    // Navigation properties
    public Category Category { get; set; } = null!;
}
