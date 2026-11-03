using ExpenseTracker.Api.Domain.Enums;

namespace ExpenseTracker.Api.Domain.Entities;

public class Category
{
    public Guid Id { get; set; }
    public Guid WalletId { get; init; }
    public string Name { get; init; } = string.Empty;
    public CategoryType Type { get; init; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }

    // Navigation properties
    public Wallet Wallet { get; init; } = null!;
    public ICollection<Transaction> Transactions { get; init; } = new List<Transaction>();
}