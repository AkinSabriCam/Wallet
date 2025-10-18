using Microsoft.EntityFrameworkCore.Storage;

namespace Application.Abstransaction;

public interface IUnitOfWork
{
    Task SaveAsync();
    
    Task<IDbContextTransaction>  StartTransactionAsync();
    
    Task CommitAsync();
    
    Task RollbackAsync();
}