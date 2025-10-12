namespace Application.DTOs;

public class CreateTransactionDto
{
    public Guid UserId { get; set; }
    
    public Guid AccountId { get; set; }
    
    public decimal Amount { get; set; }

}