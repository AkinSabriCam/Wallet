namespace Application.Abstraction;

public class CreateTransactionDto
{
    public string UserId { get; set; }
    
    public decimal Amount { get; set; }
}