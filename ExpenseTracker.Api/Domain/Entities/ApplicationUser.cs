using Microsoft.AspNetCore.Identity;

namespace ExpenseTracker.Api.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public string? FullName { get; init; }
    public DateTime CreatedAtUtc { get; init; } = DateTime.UtcNow;

    // Navigation property
    public ICollection<Wallet> Wallets { get; init; } = new List<Wallet>();
}