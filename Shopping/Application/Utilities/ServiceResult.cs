namespace Application.Utilities;

public class ServiceResult
{
    public ServiceResult()
    {
    }
    
    public List<string> ErrorMessages { get; set; }

    public bool IsSuccess { get => ErrorMessages.Count == 0; set => IsSuccess = value; }

    public static ServiceResult Failed(List<string> errorMessages)
    {
        return new ServiceResult()
        {
            ErrorMessages = errorMessages
        };
    }

    public static ServiceResult Success()
    {
        return new ServiceResult();
    }

    public static ServiceResult<T> Success<T>(T value)
    {
        return new ServiceResult<T>(value);
    }

    public static ServiceResult<T> Failed<T>(List<string> errorMessages)
    {
        return new ServiceResult<T>()
        {
            ErrorMessages = errorMessages
        };
    }
}

public class ServiceResult<T> : ServiceResult
{
    public T Result { get; set; }

    public ServiceResult(T result)
    {
        Result = result;
    }

    public ServiceResult()
    {
    }
}