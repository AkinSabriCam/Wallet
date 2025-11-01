using System.Text.Json;
using Application.Abstransaction;
using Application.Utilities;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly WalletDbContext _walletDbContext;
    private readonly HttpContext _httpContext;

    public UnitOfWork(WalletDbContext walletDbContext, IHttpContextAccessor httpContextAccessor)
    {
        _walletDbContext = walletDbContext;
        _httpContext = httpContextAccessor.HttpContext;
    }

    public async Task SaveAsync()
    {
        await _walletDbContext.SaveChangesAsync();
    }

    public async Task<IDbContextTransaction> StartTransactionAsync()
    {
        return await _walletDbContext.Database.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        await _walletDbContext.Database.CommitTransactionAsync();
    }

    public async Task RollbackAsync()
    {
        await _walletDbContext.Database.RollbackTransactionAsync();
    }

    public async Task SaveWithIdempotency<TResponse>(Func<Task<ServiceResult<TResponse>>> action) where TResponse : class
    {
        var requestId = _httpContext.Request.Headers["x-request-id"].FirstOrDefault();
        var bodyHash = _httpContext.Request.Headers["x-body-hash"].FirstOrDefault();
        var userId = _httpContext.Request.Headers["x-user-Id"].FirstOrDefault();

        if (requestId == null || bodyHash == null || userId == null)
        {
            await action.Invoke();

            await _walletDbContext.SaveChangesAsync();
            
            return;
        }
        
        await _walletDbContext.Database.BeginTransactionAsync();

        try
        {
            var httpRequestTable = _walletDbContext.Set<HttpRequestEntity>();

            httpRequestTable.Add(new HttpRequestEntity()
            {
                UserId = userId,
                BodyHash = bodyHash,
                RequestId = requestId,
                Path = _httpContext.Request.Path.ToString(),
                Status = HttpRequestEntityStatus.Pending
            });

            await _walletDbContext.SaveChangesAsync();

            var response = await action.Invoke();

            var httpRequestEntity = await _walletDbContext.Set<HttpRequestEntity>().AsQueryable()
                .FirstAsync(x=>x.RequestId == requestId && x.BodyHash == bodyHash && x.UserId == userId &&
                                   x.Path == _httpContext.Request.Path.ToString());
        
            httpRequestEntity.Status = !response.IsSuccess ? HttpRequestEntityStatus.Failed : HttpRequestEntityStatus.Completed;

            httpRequestEntity.Response = JsonSerializer.Serialize(response);

            await _walletDbContext.SaveChangesAsync();

            await _walletDbContext.Database.CommitTransactionAsync();
        }
        catch
        {
            await _walletDbContext.Database.RollbackTransactionAsync();
            
            throw;
        }
    }
    
    public async Task SaveWithIdempotencyWithoutDbTransaction<TResponse>(Func<Task<ServiceResult<TResponse>>> action)
    {
        var requestId = _httpContext.Request.Headers["x-request-id"].FirstOrDefault();
        var bodyHash = _httpContext.Request.Headers["x-body-hash"].FirstOrDefault();
        var userId = _httpContext.Request.Headers["x-user-Id"].FirstOrDefault();

        if (requestId == null || bodyHash == null || userId == null)
        {
            await action.Invoke();

            await _walletDbContext.SaveChangesAsync();
            
            return;
        }

        try
        {
            var httpRequestTable = _walletDbContext.Set<HttpRequestEntity>();

            httpRequestTable.Add(new HttpRequestEntity()
            {
                UserId = userId,
                BodyHash = bodyHash,
                RequestId = requestId,
                Path = _httpContext.Request.Path.ToString(),
                Status = HttpRequestEntityStatus.Pending
            });

            await _walletDbContext.SaveChangesAsync();

            var response = await action.Invoke();

            var httpRequestEntity = await _walletDbContext.Set<HttpRequestEntity>().AsQueryable()
                .FirstAsync(x=>x.RequestId == requestId && x.BodyHash == bodyHash && x.UserId == userId &&
                               x.Path == _httpContext.Request.Path.ToString());
        
            httpRequestEntity.Status = !response.IsSuccess ? HttpRequestEntityStatus.Failed : HttpRequestEntityStatus.Completed;

            httpRequestEntity.Response = JsonSerializer.Serialize(response);

            await _walletDbContext.SaveChangesAsync();
        }
        catch
        {
            
            throw;
        }
    }
}