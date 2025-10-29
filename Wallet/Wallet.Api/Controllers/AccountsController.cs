using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Wallet.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountsController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountsController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpGet]
    [Route("get-by-id")]
    public async Task<IActionResult> Get(Guid id)
    { 
        return Ok(await _accountService.GetAccount(id));
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(CreateAccountDto dto)
    {
        HttpContext.Request.Headers.TryGetValue("x-user-id", out var userId);
        dto.UserId = Guid.Parse(userId.ToString());
        
        return Ok(await _accountService.AddAccount(dto));
    }
}