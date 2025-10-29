using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Wallet.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionsController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }
    
    
    [HttpGet]
    [Route("get-by-accountId")]
    public async Task<IActionResult> Get(Guid accountId)
    {
        return Ok(await _transactionService.GetTransactions(accountId));
    }
    
    [HttpPost("pay-by-wallet")]
    public async Task<IActionResult> Create(PaymentDto dto)
    {
        HttpContext.Request.Headers.TryGetValue("x-user-id", out var userId);
        dto.UserId = Guid.Parse(userId);
        throw new Exception();
        return Ok(await _transactionService.Pay(dto));
    }
    
    [HttpPost("cancel-payment")]
    public async Task<IActionResult> CancelTransaction(PaymentDto dto)
    {
        HttpContext.Request.Headers.TryGetValue("x-user-id", out var userId);
        dto.UserId = Guid.Parse(userId);
        
        return Ok(await _transactionService.CancelPayment(dto));
    }
}