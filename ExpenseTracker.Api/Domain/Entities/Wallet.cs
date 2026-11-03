namespace ExpenseTracker.Api.Domain.Entities;

public class Wallet
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Currency { get; init; } = "IDR";
    public decimal InitialBalance { get; init; }
    public string? Description { get; init; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }

    // Navigation properties
    public ApplicationUser User { get; init; } = null!;
    public ICollection<Category> Categories { get; init; } = new List<Category>();
}