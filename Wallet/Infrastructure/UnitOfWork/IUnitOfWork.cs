namespace Infrastructure.UnitOfWork;

public interface IUnitOfWork
{
    Task SaveAsync();
    
    Task  StartTransactionAsync();
    
    Task CommitAsync();
    
    Task RollbackAsync();
}