namespace Domain.Entities;

public class TransactionEntity
{
    public TransactionEntity()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }
    
    public Guid Id { get; set; }
    
    public Guid UserId { get; set; }
    
    public Guid AccountId { get; set; }
    
    public decimal Amount { get; set; }
    
    public DateTime CreatedAt { get; set; }
}