using Application.Services;
using Application.Services.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Shopping.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShoppingBasketController(IShoppingBasketService shoppingBasketService) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        return Ok(await shoppingBasketService.GetShoppingBasketAsync(id));
    }
   
    [HttpPost]
    public async Task<IActionResult> Create(AddShoppingBasketDto dto)
    {
        await shoppingBasketService.Create(dto);
            
        return Ok();
    }
}