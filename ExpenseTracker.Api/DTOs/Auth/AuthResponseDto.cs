namespace ExpenseTracker.Api.DTOs.Auth;

public record AuthResponseDto
{
    public string Token { get; init; } = string.Empty;
    public UserResponseDto User { get; init; } = null!;
}
