namespace Application.Utilities;

public class ServiceResult
{
    protected ServiceResult(List<string> errorMessages)
    {
        ErrorMessages = errorMessages;
    }
    
    public ServiceResult()
    {
    }
    
    public List<string> ErrorMessages { get; set; } = new();
    
    public bool IsSuccess => ErrorMessages.Count == 0;

    public static ServiceResult Success()
    {
        return new ServiceResult();
    }

    public static ServiceResult Fail(List<string> errorMessages)
    {
        return new ServiceResult(errorMessages.ToList());
    }
    
    public static ServiceResult<T> Success<T>(T data)
    {
        return new ServiceResult<T>(data);
    }
    
    public static ServiceResult<T> Fail<T>(List<string> errorMessages)
    {
        return new ServiceResult<T>(errorMessages.ToList());
    }
}

public class ServiceResult<T> : ServiceResult
{
    public T Data { get; set; }

    public ServiceResult(T data)
    {
        Data = data;
    }
    
    public ServiceResult(List<string> errorMessages)
    {
        ErrorMessages = errorMessages;
    }

    public ServiceResult()
    {
        
    }
}