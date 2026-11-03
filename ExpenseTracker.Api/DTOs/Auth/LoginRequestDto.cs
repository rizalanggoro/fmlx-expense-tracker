namespace ExpenseTracker.Api.DTOs.Auth;

public record LoginRequestDto
{
    public string EmailOrUserName { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}
