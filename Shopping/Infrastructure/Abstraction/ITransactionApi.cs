namespace Infrastructure.Abstraction;

public interface ITransactionApi
{
    Task<bool> Pay(DecreaseWalletAmount model);
    
    Task<bool> CancelPayment(DecreaseWalletAmount model);

}