using Application.Services.DTOs;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Shopping.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController(IProductRepository productRepository) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return Ok(await productRepository.GetProducts());
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        return Ok(await productRepository.GetProduct(id));
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(AddProductDto productDto)
    {
        await productRepository.AddProduct(new ProductEntity()
        {
            Name = productDto.Name,
            Price = productDto.Price,
        });
            
        return Ok();
    }
}