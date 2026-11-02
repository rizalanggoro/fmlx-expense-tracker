namespace ExpenseTracker.Api.Common;

public class ServiceResult<T>
{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public List<string>? Errors { get; set; }
    public int StatusCode { get; set; }

    public static ServiceResult<T> Success(T data, string? message = null, int statusCode = 200)
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

    public static ServiceResult<T> Failure(string message, List<string>? errors = null, int statusCode = 400)
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

    public static ServiceResult<T> NotFound(string message = "Resource not found")
    {
        return Failure(message, null, 404);
    }

    public static ServiceResult<T> Forbidden(string message = "You do not have permission to access this resource")
    {
        return Failure(message, null, 403);
    }
}
