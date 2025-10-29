using Domain.Entities;

namespace Domain.Repositories;

public interface IShoppingBasketRepository
{
    Task<ShoppingBasketEntity> GetShoppingBasket(Guid shoppingBasketId);
    
    Task<ShoppingBasketEntity> Add(ShoppingBasketEntity shoppingBasketEntity);
}