namespace UIShared.Models;

public class Response
{
    public bool IsSuccess { get; protected set; }
    public ProblemDetails? Error { get; protected set; }
    public bool IsFailure => !IsSuccess;
    public string ErrorMessage => Error == null
        ? string.Empty
        : Error.Detail;

    protected Response() { }

    public static Response Success() => new Response
    {
        IsSuccess = true,
        Error = null
    };
    public static Response Failure(ProblemDetails error) => new Response
    {
        IsSuccess = false,
        Error = error
    };
}

public class Response<T> : Response
{
    private T _value;
    public T Value
    {
        get
        {
            if (!IsSuccess)
            {
                throw new InvalidOperationException("Cannot access Value when the response is a failure.");
            }
            return _value;
        }
        private set
        {
            _value = value;
        }
    }

    private Response() { }

    public static Response<T> Success(T value)
    {
        return new Response<T>
        {
            IsSuccess = true,
            Value = value,
            Error = null
        };
    }

    public static new Response<T> Failure(ProblemDetails problem)
    {
        return new Response<T>
        {
            IsSuccess = false,
            Value = default!,
            Error = problem
        };
    }
}