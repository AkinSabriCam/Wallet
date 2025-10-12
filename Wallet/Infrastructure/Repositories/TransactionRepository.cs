using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly DbSet<TransactionEntity> _transactions;

    public TransactionRepository(WalletDbContext context)
    {
        _transactions = context.Set<TransactionEntity>();
    }
    
    public async Task<List<TransactionEntity>> GetTransactions(Guid accountId)
    {
        return await _transactions.Where(x=>x.AccountId == accountId).ToListAsync();
    }

    public async Task<TransactionEntity> Add(TransactionEntity transaction)
    {
        await _transactions.AddAsync(transaction);

        return transaction;
    }
}