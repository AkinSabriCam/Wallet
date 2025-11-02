using Application.Utilities;

namespace Application.Abstraction;

public interface ITransactionApi
{
    Task<ServiceResult> Pay(CreateTransactionDto model);
    
    Task<ServiceResult> CancelPayment(CreateTransactionDto model);
}