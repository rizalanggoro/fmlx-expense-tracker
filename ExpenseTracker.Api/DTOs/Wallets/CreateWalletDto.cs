namespace ExpenseTracker.Api.DTOs.Wallets;

public record CreateWalletDto(
    string? Description
)
{
    public string Name => string.Empty;
    public string Currency => "IDR";
    public decimal InitialBalance => 0.00m;
}