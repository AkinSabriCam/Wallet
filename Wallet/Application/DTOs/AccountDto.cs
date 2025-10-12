using Domain.Entities;

namespace Application.DTOs;

public class AccountDto
{
    public Guid Id { get; set; }
    
    public Guid UserId { get; set; }
    
    public Currency Currency { get; set; }
    
    public decimal Amount { get; set; }
    
    public DateTime CreatedAt { get; set; }
}