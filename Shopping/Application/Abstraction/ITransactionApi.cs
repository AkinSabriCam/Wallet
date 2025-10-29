namespace Application.Abstraction;

public interface ITransactionApi
{
    Task<bool> Pay(CreateTransactionDto model);
    
    Task<bool> CancelPayment(CreateTransactionDto model);

}