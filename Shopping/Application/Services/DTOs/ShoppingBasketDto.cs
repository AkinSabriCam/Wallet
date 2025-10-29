namespace Application.Services.DTOs;

public class ShoppingBasketDto
{
    public Guid Id { get; set; }
    
    public decimal TotalAmount { get; set; }
    
    public string UserId { get; set; }
}