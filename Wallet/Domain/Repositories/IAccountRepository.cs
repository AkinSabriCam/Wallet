using Domain.Entities;

namespace Domain.Repositories;

public interface IAccountRepository
{
    Task<List<AccountEntity>> GetAccounts(Guid userId);
    
    Task<AccountEntity?> GetAccountById(Guid id);

    Task<AccountEntity> Add(AccountEntity entity);
    
    Task<bool> IsExist(Guid userId, Currency currency);
}