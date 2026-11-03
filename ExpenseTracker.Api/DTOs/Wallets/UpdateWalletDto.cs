namespace ExpenseTracker.Api.DTOs.Wallets;

public record UpdateWalletDto
{
    public string Name { get; init; } = string.Empty;
    public string Currency { get; init; } = "IDR";
    public string? Description { get; init; }
}
