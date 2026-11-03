namespace ExpenseTracker.Api.Domain.Entities;

public class Transaction
{
    public Guid Id { get; set; }
    public Guid CategoryId { get; init; }
    public decimal Amount { get; init; }
    public DateTime TransactionDate { get; init; }
    public string? Description { get; init; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }

    // Navigation properties
    public Category Category { get; set; } = null!;
}

