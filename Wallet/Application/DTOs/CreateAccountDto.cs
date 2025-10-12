using Domain.Entities;

namespace Application.DTOs;

public class CreateAccountDto
{
    public Guid UserId { get; set; }
    
    public Currency Currency { get; set; }
    
    public decimal Amount { get; set; }
}