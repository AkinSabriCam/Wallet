using Api.Models;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

public class ShoppingBasketController(IShoppingBasketRepository shoppingBasketRepository) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        return Ok(await shoppingBasketRepository.GetShoppingBasket(id));
    }
   
    [HttpPost]
    public async Task<IActionResult> Create(AddShoppingBasketDto dto)
    {
        await shoppingBasketRepository.Add(new ShoppingBasketEntity()
        {
            TotalAmount = dto.TotalAmount,
            UserId = dto.UserId,
        });
            
        return Ok();
    }
}