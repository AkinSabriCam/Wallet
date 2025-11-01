namespace Infrastructure.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly WalletDbContext _walletDbContext;

    public UnitOfWork(WalletDbContext walletDbContext)
    {
        _walletDbContext = walletDbContext;
    }

    public async Task SaveAsync()
    {
        await _walletDbContext.SaveChangesAsync();
    }

    public async Task StartTransactionAsync()
    {
        await _walletDbContext.Database.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        await _walletDbContext.Database.CommitTransactionAsync();
    }

    public async Task RollbackAsync()
    {
        await _walletDbContext.Database.RollbackTransactionAsync();
    }
}