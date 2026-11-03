namespace ExpenseTracker.Api.DTOs.Auth;

public record RegisterRequestDto(
    string? FullName
)
{
    public string Email => string.Empty;
    public string UserName => string.Empty;
    public string Password => string.Empty;
}