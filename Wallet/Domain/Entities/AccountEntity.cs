namespace Domain.Entities;

public class AccountEntity
{
    public AccountEntity()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }
    
    public Guid Id { get; set; }
    
    public Guid UserId { get; set; }
    
    public decimal Amount { get; set; }
    
    public Currency Currency { get; set; }
    
    public byte[] RowVersion { get; set; }
    
    public DateTime CreatedAt { get; set; }
}