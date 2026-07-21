namespace RestaurantPOS.Core.Common;

public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Data { get; }
    public string? Error { get; }
    public List<string> Errors { get; }

    private Result(T? data, bool isSuccess, string? error = null, List<string>? errors = null)
    {
        Data = data;
        IsSuccess = isSuccess;
        Error = error;
        Errors = errors ?? new List<string>();
    }

    public static Result<T> Success(T data) => new(data, true);
    public static Result<T> Failure(string error) => new(default, false, error);
    public static Result<T> Failure(List<string> errors) => new(default, false, errors: errors);
}
