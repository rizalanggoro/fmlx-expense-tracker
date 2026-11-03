namespace ExpenseTracker.Api.DTOs.Auth;

public record UserResponseDto
{
    public string Id { get; init; } = string.Empty;
    public string UserName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? FullName { get; init; }
    public DateTime CreatedAtUtc { get; init; }
}
