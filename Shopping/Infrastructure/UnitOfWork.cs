using Application.Abstraction;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure;

public class UnitOfWork(ShoppingDbContext dbContext) : IUnitOfWork
{
    public async Task SaveAsync()
    {
        await dbContext.SaveChangesAsync();
    }

    public async Task RollbackAsync()
    {
        await dbContext.Database.RollbackTransactionAsync();
    }

    public async Task CommitAsync()
    {
        await dbContext.Database.CommitTransactionAsync();
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await dbContext.Database.BeginTransactionAsync();
    }
}