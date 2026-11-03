namespace ExpenseTracker.Api.DTOs.Auth;

public record LoginRequestDto
{
    public string EmailOrUserName => string.Empty;
    public string Password => string.Empty;
}