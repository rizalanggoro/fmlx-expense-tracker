namespace ExpenseTracker.Api.DTOs.Wallets;

public record CreateWalletDto
{
    public string Name { get; init; } = string.Empty;
    public string Currency { get; init; } = "IDR";
    public decimal InitialBalance { get; init; } = 0.00m;
    public string? Description { get; init; }
}
