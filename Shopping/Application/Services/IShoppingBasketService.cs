using Application.Services.DTOs;
using Application.Utilities;

namespace Application.Services;

public interface IShoppingBasketService
{
    Task<ShoppingBasketDto> GetShoppingBasketAsync(Guid id);
    
    Task<ServiceResult> Create(AddShoppingBasketDto dto);
}