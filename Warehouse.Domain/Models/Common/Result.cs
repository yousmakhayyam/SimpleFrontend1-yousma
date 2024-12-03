namespace Warehouse.Domain.Models.Common;

public class Result
{
    internal Result(bool succeeded, IEnumerable<string> errors)
    {
        Succeeded = succeeded;
        Errors = errors.ToArray();
    }

    public bool Succeeded { get; init; }

    public string[] Errors { get; init; }

    public static Result Success()
    {
        return new Result(true, Array.Empty<string>());
    }

    public static Result Failure(IEnumerable<string> errors)
    {
        return new Result(false, errors);
    }

    public static Result Failure(string error)
    {
        return new Result(false, new[] { error });
    }
}

public class Result<T> : Result
{
    public T? Value { get; }

    internal Result(T value, bool succeeded, IEnumerable<string> errors) : base(succeeded, errors)
    {
        Value = value;
    }

    public static Result<T> Success(T value)
    {
        return new Result<T>(value, true, Array.Empty<string>());
    }

    public static Result<T> Failure(IEnumerable<string> errors)
    {
        return new Result<T>(default!, false, errors);
    }

    public static Result<T> Failure(string error)
    {
        return new Result<T>(default!, false, new[] { error });
    }
}