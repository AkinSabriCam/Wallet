namespace Application.DTOs;

public class TransactionDto
{
    public Guid Id { get; set; }
    
    public Guid UserId { get; set; }
    
    public Guid AccountId { get; set; }
    
    public decimal Amount { get; set; }
    
    public DateTime CreatedAt { get; set; }
}