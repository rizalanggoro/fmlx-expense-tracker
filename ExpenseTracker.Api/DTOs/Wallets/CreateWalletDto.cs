namespace ExpenseTracker.Api.DTOs.Wallets;

public class CreateWalletDto
{
    public string Name { get; set; } = string.Empty;
    public string Currency { get; set; } = "IDR";
    public decimal InitialBalance { get; set; } = 0.00m;
    public string? Description { get; set; }
}
