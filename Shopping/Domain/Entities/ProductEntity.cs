namespace Domain.Entities;

public class ProductEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public string Name { get; set; }
    
    public decimal Price { get; set; }
    
    public List<ShoppingBasketProductEntity> Baskets { get; set; } = new();
}