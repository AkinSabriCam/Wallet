namespace Domain.Entities;

public class HttpRequestEntity
{
    public string UserId { get; set; }
    
    public string Path { get; set; }
    
    public string RequestId { get; set; }
    
    public string BodyHash { get; set; }
}