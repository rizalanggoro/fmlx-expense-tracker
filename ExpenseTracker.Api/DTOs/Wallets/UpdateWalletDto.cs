namespace ExpenseTracker.Api.DTOs.Wallets;

public class UpdateWalletDto
{
    public string Name { get; set; } = string.Empty;
    public string Currency { get; set; } = "IDR";
    public string? Description { get; set; }
}

