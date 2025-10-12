using Domain.Entities;

namespace Domain.Repositories;

public interface ITransactionRepository
{
    Task<List<TransactionEntity>> GetTransactions(Guid accountId);
    
    Task<TransactionEntity> Add(TransactionEntity transaction);
}