using Application.DTOs;
using Application.Utilities;

namespace Application.Services;

public interface ITransactionService
{
    Task<ServiceResult<List<TransactionDto>>> GetTransactions(Guid accountId);
    
    Task<ServiceResult<TransactionDto>> Pay(PaymentDto dto);
    
    Task<ServiceResult<TransactionDto>> CancelPayment(PaymentDto dto);

}