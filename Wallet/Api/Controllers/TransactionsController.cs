using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
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
    
    [HttpPost]
    public async Task<IActionResult> Create(CreateTransactionDto dto)
    {
        return Ok(await _transactionService.Add(dto));
    }
}