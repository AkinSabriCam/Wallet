using Application.DTOs;

namespace Application.Services;

public interface ITransactionService
{
    Task<List<TransactionDto>> GetTransactions(Guid accountId);
    
    Task<TransactionDto> Add(CreateTransactionDto dto);
}