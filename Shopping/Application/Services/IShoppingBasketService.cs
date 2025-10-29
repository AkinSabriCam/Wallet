using Application.Services.DTOs;

namespace Application.Services;

public interface IShoppingBasketService
{
    Task<ShoppingBasketDto> GetShoppingBasketAsync(Guid id);
    
    Task Create(AddShoppingBasketDto dto);
}