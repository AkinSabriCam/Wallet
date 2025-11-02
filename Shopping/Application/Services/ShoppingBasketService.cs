using Application.Abstraction;
using Application.Services.DTOs;
using Application.Utilities;
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

    public async Task<ServiceResult> Create(AddShoppingBasketDto dto)
    {
        var paymentResult = await transactionApi.Pay(new CreateTransactionDto()
        {
            AccountId = dto.AccountId,
            Amount = dto.TotalAmount,
            UserId = dto.UserId,
        });

        if (paymentResult.IsSuccess)
        {
            try
            {
                await shoppingBasketRepository.Add(new ShoppingBasketEntity()
                {
                    TotalAmount = dto.TotalAmount,
                    UserId = dto.UserId,
                    Products = dto.ProductIds.Select(x=> new ShoppingBasketProductEntity()
                    {
                        ProductId = x
                    }).ToList()
                });

                await unitOfWork.SaveAsync();
            }
            catch (Exception ex)
            {
                var cancelResult = await transactionApi.CancelPayment(new CreateTransactionDto()
                {
                    UserId = dto.UserId,
                    Amount = dto.TotalAmount,
                });

                if (!cancelResult.IsSuccess)
                {
                    Console.WriteLine("Could not cancelled the payment!");
                    //todo: take action like save this amount and info the wallet api 
                }

                throw;
            }
        }

        return paymentResult;
    }
}