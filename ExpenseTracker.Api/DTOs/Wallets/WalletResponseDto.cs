namespace ExpenseTracker.Api.DTOs.Wallets;

public record WalletResponseDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Currency { get; init; } = "IDR";
    public decimal InitialBalance { get; init; }
    public decimal CurrentBalance { get; init; }
    public decimal TotalIncome { get; init; }
    public decimal TotalExpense { get; init; }
    public string? Description { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}
