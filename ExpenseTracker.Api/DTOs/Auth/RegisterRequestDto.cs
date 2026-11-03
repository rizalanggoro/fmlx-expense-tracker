namespace ExpenseTracker.Api.DTOs.Auth;

public record RegisterRequestDto
{
    public string UserName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string? FullName { get; init; }
}
