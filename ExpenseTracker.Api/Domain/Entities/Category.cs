using ExpenseTracker.Api.Domain.Enums;

namespace ExpenseTracker.Api.Domain.Entities;

public class Category
{
    public Guid Id { get; set; }
    public Guid WalletId { get; set; }
    public string Name { get; set; } = string.Empty;
    public CategoryType Type { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }

    // Navigation properties
    public Wallet Wallet { get; set; } = null!;
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}

