namespace ExpenseTracker.Api.DTOs.Wallets;

public record UpdateWalletDto(
    string? Description
)
{
    public string Name => string.Empty;
    public string Currency => "IDR";
}