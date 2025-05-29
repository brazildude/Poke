namespace Poke.Server.Infrastructure.GameLogic;

public class ResultLogic<T>
{
    public bool IsSuccess { get; }
    public string? Error { get; }
    public T? Value { get; }

    private ResultLogic(bool isSuccess, T? value, string? error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    public static ResultLogic<T> Success(T value) => new(true, value, null);
    public static ResultLogic<T> Failure(string error) => new(false, default, error);
}
