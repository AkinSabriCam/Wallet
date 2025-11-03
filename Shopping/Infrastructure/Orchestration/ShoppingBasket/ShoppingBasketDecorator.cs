using Application.Services;
using Application.Services.DTOs;
using Application.Utilities;

namespace Infrastructure.Orchestration.ShoppingBasket;

public class ShoppingBasketDecorator(
    IShoppingBasketService inner,
    Application.Abstraction.IUnitOfWork unitOfWork)
    : Infrastructure.Orchestration.BaseDecorator(unitOfWork), IShoppingBasketService
{
    public Task<ShoppingBasketDto> GetShoppingBasketAsync(Guid id)
    {
        return inner.GetShoppingBasketAsync(id);
    }

    public async Task<ServiceResult> Create(AddShoppingBasketDto dto)
    {
        return await Invoke(() => inner.Create(dto));
    }
}


