using Application.Abstraction;
using Application.Services.DTOs;
using Domain.Entities;
using Domain.Repositories;

namespace Application.Services;

public class ShoppingBasketService(
    IShoppingBasketRepository shoppingBasketRepository,
    ITransactionApi transactionApi,IUnitOfWork unitOfWork) : IShoppingBasketService
{
    public async Task<ShoppingBasketDto> GetShoppingBasketAsync(Guid id)
    {
        var entity = await shoppingBasketRepository.GetShoppingBasket(id);

        return new ShoppingBasketDto()
        {
            Id = entity.Id,
            TotalAmount = entity.TotalAmount,
            UserId = entity.UserId
        };
    }

    public async Task Create(AddShoppingBasketDto dto)
    {
        var isPaid = await transactionApi.Pay(new CreateTransactionDto()
        {
            Amount = dto.TotalAmount,
            UserId = dto.UserId,
        });

        if (isPaid)
        {
            try
            {
                await shoppingBasketRepository.Add(new ShoppingBasketEntity()
                {
                    TotalAmount = dto.TotalAmount,
                    UserId = dto.UserId
                });

                await unitOfWork.SaveAsync();
            }
            catch (Exception ex)
            {
                var isCancelled = await transactionApi.CancelPayment(new CreateTransactionDto()
                {
                    UserId = dto.UserId,
                    Amount = dto.TotalAmount,
                });

                if (!isCancelled)
                {
                    Console.WriteLine("Could not cancelled the payment!");
                    //todo: take action like save this amount and info the wallet api 
                }

                throw;
            }
        }
    }
}