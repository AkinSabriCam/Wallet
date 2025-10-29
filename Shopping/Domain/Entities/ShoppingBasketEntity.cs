namespace Domain.Entities;

public class ShoppingBasketEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public decimal TotalAmount { get; set; }
    
    public string UserId { get; set; }

    public List<ShoppingBasketProductEntity> Products { get; set; } = new();
}