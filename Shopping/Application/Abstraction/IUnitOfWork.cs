using Microsoft.EntityFrameworkCore.Storage;

namespace Application.Abstraction;

public interface IUnitOfWork
{
    Task SaveAsync();
    
    Task RollbackAsync();
    
    Task CommitAsync();
    
    Task<IDbContextTransaction> BeginTransactionAsync();
}