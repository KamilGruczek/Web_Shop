namespace Web_Shop.Application.Common;

public class ServiceResponse<T> : ServiceResponse
{
    public ServiceResponse(T data) : base(true)
    {
        Data = data;
    }

    public ServiceResponse(bool isSuccess, string message, bool logError = false) : base(isSuccess, message, logError)
    {
        Data = default;
    }

    public T? Data { get; set; }
}

public class ServiceResponse
{
    public ServiceResponse(bool isSuccess)
    {
        IsSuccess = isSuccess;
        Message = null;
    }

    public ServiceResponse(bool isSuccess, string message, bool logError = false)
    {
        IsSuccess = isSuccess;
        Message = message;
        LogMessage = logError;
    }

    public bool IsSuccess { get; }
    public string? Message { get; }
    public bool LogMessage { get; }
}