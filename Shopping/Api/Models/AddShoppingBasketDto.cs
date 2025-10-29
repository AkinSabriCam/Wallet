namespace Api.Models;

public class AddShoppingBasketDto
{
    public decimal TotalAmount { get; set; }
    
    public Guid UserId { get; set; }
}