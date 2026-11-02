using Microsoft.AspNetCore.Identity;

namespace ExpenseTracker.Api.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    // Navigation property
    public ICollection<Wallet> Wallets { get; set; } = new List<Wallet>();
}
