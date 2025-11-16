namespace Taskly.Core.Functional;

public class Result 
{
    public Error? Error { get; init; }

    protected Result(Error? error) => Error = error;

    protected Result() { }

    public bool IsError => Error is not null;

    public static Result Success() => new();
    public static Result Failure(Error error) => new(error);
    
    public static implicit operator Result(Error error) => new (error);

    public override string ToString()
        => IsError ? $"Error: {Error}" : "Success";
}

public sealed class Result<T> : Result
{
    public T? Value { get; init; }

    private Result(T value) => Value = value;
    private Result(Error error) : base(error) { }

    public static Result<T> Success(T value) => new(value);

    public static implicit operator Result<T>(T value)
        => Success(value);

    public static implicit operator Result<T>(Error error)
        => new(error);

    public override string ToString()
        => IsError ? $"Error: {Error}" : $"Success: {Value}";
}