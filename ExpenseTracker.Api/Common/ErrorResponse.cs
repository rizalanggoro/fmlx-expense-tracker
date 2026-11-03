namespace ExpenseTracker.Api.Common;

public class ErrorResponse(
    string message,
    List<string>? errors = null
)
{
    public string Message { get; init; } = message;
    public List<string>? Errors { get; init; } = errors ?? [message];
}