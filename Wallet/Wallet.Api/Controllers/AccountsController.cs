using Application.DTOs;
using Infrastructure.Orchestration.Account;
using Microsoft.AspNetCore.Mvc;

namespace Wallet.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountsController : ControllerBase
{
    private readonly IAccountDecorator _accountDecorator;

    public AccountsController(IAccountDecorator accountDecorator)
    {
        _accountDecorator = accountDecorator;
    }

    [HttpGet]
    [Route("get-by-id")]
    public async Task<IActionResult> Get(Guid id)
    { 
        return Ok(await _accountDecorator.GetAccount(id));
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(CreateAccountDto dto)
    {
        HttpContext.Request.Headers.TryGetValue("x-user-id", out var userId);
        dto.UserId = Guid.Parse(userId.ToString());
        
        return Ok(await _accountDecorator.AddAccount(dto));

    }
}