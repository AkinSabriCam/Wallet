namespace Application.Services.DTOs;

public class AddShoppingBasketDto
{
    public Guid AccountId { get; set; }

    public List<Guid> ProductIds { get; set; } = new();
    
    public decimal TotalAmount { get; set; }
    
    public string UserId { get; set; }
}