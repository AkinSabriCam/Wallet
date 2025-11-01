using Application.DTOs;
using Infrastructure.Orchestration.Transaction;
using Microsoft.AspNetCore.Mvc;

namespace Wallet.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionDecorator _transactionDecorator;

    public TransactionsController(ITransactionDecorator transactionDecorator)
    {
        _transactionDecorator = transactionDecorator;
    }
    
    [HttpGet]
    [Route("get-by-accountId")]
    public async Task<IActionResult> Get(Guid accountId)
    {
        return Ok(await _transactionDecorator.GetTransactions(accountId));
    }
    
    [HttpPost("pay-by-wallet")]
    public async Task<IActionResult> Create(PaymentDto dto)
    {
        HttpContext.Request.Headers.TryGetValue("x-user-id", out var userId);
        dto.UserId = Guid.Parse(userId);

        return Ok(await _transactionDecorator.Pay(dto));
    }
    
    [HttpPost("cancel-payment")]
    public async Task<IActionResult> CancelTransaction(PaymentDto dto)
    {
        HttpContext.Request.Headers.TryGetValue("x-user-id", out var userId);
        dto.UserId = Guid.Parse(userId);
        
        return Ok(await _transactionDecorator.CancelPayment(dto));
    }
}