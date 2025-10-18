using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly DbSet<AccountEntity> _accounts;

    public AccountRepository(WalletDbContext context)
    {
        _accounts = context.Set<AccountEntity>();
    }
    
    public async Task<List<AccountEntity>> GetAccounts(Guid userId)
    {
        return await _accounts.Where(x => x.UserId == userId).ToListAsync();
    }

    public async Task<AccountEntity?> GetAccountById(Guid id)
    {
        return await _accounts.AsTracking().FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<AccountEntity> Add(AccountEntity account)
    {
        await _accounts.AddAsync(account);

        return account;
    }

    public async Task<bool> IsExist(Guid userId, Currency currency)
    {
        return await _accounts.AnyAsync(x => x.UserId == userId && x.Currency == currency);
    }
}