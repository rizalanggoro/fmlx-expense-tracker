namespace ExpenseTracker.Api.Domain.Entities;

public class Wallet
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Currency { get; set; } = "IDR";
    public decimal InitialBalance { get; set; } = 0.00m;
    public string? Description { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }

    // Navigation properties
    public ApplicationUser User { get; set; } = null!;
    public ICollection<Category> Categories { get; set; } = new List<Category>();
}

