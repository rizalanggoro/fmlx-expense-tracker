namespace ExpenseTracker.Api.Common;

public class ServiceResult<T>
{
    public bool IsSuccess { get; private init; }
    public T? Data { get; private init; }
    public string? Message { get; private init; }
    public List<string>? Errors { get; private init; }
    public int StatusCode { get; private init; }

    public static ServiceResult<T> Success(
        T data,
        string? message = null,
        int statusCode = 200
    )
    {
        return new ServiceResult<T>
        {
            IsSuccess = true,
            Data = data,
            Message = message,
            Errors = null,
            StatusCode = statusCode
        };
    }

    public static ServiceResult<T> Failure(
        string message,
        int statusCode = 400,
        List<string>? errors = null
    )
    {
        return new ServiceResult<T>
        {
            IsSuccess = false,
            Data = default,
            Message = message,
            Errors = errors ?? [message],
            StatusCode = statusCode
        };
    }
}