using System.Text.Json;
using Application.Utilities;
using Domain.Entities;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Orchestration;

public class BaseDecorator
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly WalletDbContext _walletDbContext;
    private readonly HttpContext _httpContext;

    public BaseDecorator(IUnitOfWork unitOfWork, WalletDbContext walletDbContext, IHttpContextAccessor httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _walletDbContext = walletDbContext;
        _httpContext = httpContextAccessor.HttpContext;
    }

    public async Task<ServiceResult> Invoke(Func<Task<ServiceResult>> func, bool isIdempotent)
    {
        if (isIdempotent)
        {
            return await PersistDbWithIdempotent(func);
        }
        return await PersistDb(func);
    }
    
    public async Task<ServiceResult<TResult>> Invoke<TResult>(Func<Task<ServiceResult<TResult>>> func, bool isIdempotent)
    {
        if (isIdempotent)
        {
            return await PersistDbWithIdempotent(func);
        }
        return await PersistDb(func);
    }

    #region  PersistDb

    private async Task<ServiceResult<TResult>> PersistDb<TResult>(Func<Task<ServiceResult<TResult>>> func)
    {
        await _unitOfWork.StartTransactionAsync();
    
        try
        {
            var result = await func.Invoke();;
    
            if (!result.IsSuccess)
            {
                return result;
            }
            
            await _unitOfWork.SaveAsync();
    
            await _unitOfWork.CommitAsync();
    
            return result;
        }
        catch (DbUpdateConcurrencyException)
        {
            await _unitOfWork.RollbackAsync();
    
            return await PersistDb(func);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            
            throw;
        }
    }
    
    private async Task<ServiceResult> PersistDb(Func<Task<ServiceResult>> func)
    {
        await _unitOfWork.StartTransactionAsync();
    
        try
        {
            var result = await func.Invoke();;
    
            if (!result.IsSuccess)
            {
                return result;
            }
            
            await _unitOfWork.SaveAsync();
    
            await _unitOfWork.CommitAsync();
    
            return result;
        }
        catch (DbUpdateConcurrencyException)
        {
            await _unitOfWork.RollbackAsync();
    
            return await PersistDb(func);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            
            throw;
        }
    }


    #endregion
    
    #region PersistDbWithIdempotent

    private async Task<ServiceResult<TResult>> PersistDbWithIdempotent<TResult>(Func<Task<ServiceResult<TResult>>> func)
    {
        await _unitOfWork.StartTransactionAsync();

        try
        {
            var userId = _httpContext.Request.Headers["x-user-id"].ToString();
            var requestId = _httpContext.Request.Headers["x-request-id"].ToString();
            var bodyHash = _httpContext.Request.Headers["x-body-hash"].ToString();

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(requestId) || string.IsNullOrEmpty(bodyHash))
            {
                throw new Exception("IdempotencyCheck failed | Need to provide body, request id and user-id in header");
            }

            var httpRequestTable = _walletDbContext.Set<HttpRequestEntity>();

            var idempotencyRecord = await httpRequestTable.FirstOrDefaultAsync(x =>
                x.RequestId == requestId && x.UserId == userId && x.BodyHash == bodyHash &&
                x.Path == _httpContext.Request.Path.ToString());

            if (idempotencyRecord != null)
            {
                if (idempotencyRecord.Status == HttpRequestEntityStatus.Pending)
                {
                    throw new Exception("This request has been already sent. | Try it with different request-id");
                }

                return JsonSerializer.Deserialize<ServiceResult<TResult>>(idempotencyRecord.Response);
            }

            await httpRequestTable.AddAsync(new HttpRequestEntity()
            {
                UserId = userId,
                Path = _httpContext.Request.Path,
                BodyHash = bodyHash,
                RequestId = requestId
            });

            await _unitOfWork.SaveAsync();

            var result = await func.Invoke();;

            if (!result.IsSuccess)
            {
                await _unitOfWork.RollbackAsync();

                return result;
            }

            await _unitOfWork.SaveAsync();

            var httpRequestEntity = await httpRequestTable.FirstAsync(x =>
                x.RequestId == requestId && x.UserId == userId && x.BodyHash == bodyHash &&
                x.Path == _httpContext.Request.Path.ToString());

            httpRequestEntity.Response = JsonSerializer.Serialize(result);
            httpRequestEntity.Status =
                result.IsSuccess ? HttpRequestEntityStatus.Completed : HttpRequestEntityStatus.Failed;

            await _unitOfWork.SaveAsync();

            await _unitOfWork.CommitAsync();

            return result;
        }
        catch (DbUpdateConcurrencyException)
        {
            await _unitOfWork.RollbackAsync();
    
            _walletDbContext.ChangeTracker.Clear();
            
            return await PersistDbWithIdempotent(func);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();

            throw;
        }
    }

    private async Task<ServiceResult> PersistDbWithIdempotent(Func<Task<ServiceResult>> func)
    {
        await _unitOfWork.StartTransactionAsync();

        try
        {
            var userId = _httpContext.Request.Headers["x-user-id"];
            var requestId = _httpContext.Request.Headers["x-request-id"];
            var bodyHash = _httpContext.Request.Headers["x-body-hash"];

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(requestId) || string.IsNullOrEmpty(bodyHash))
            {
                throw new Exception("IdempotencyCheck failed | Need to provide body, request id and user-id in header");
            }

            var httpRequestTable = _walletDbContext.Set<HttpRequestEntity>();

            var idempotencyRecord = await httpRequestTable.FirstOrDefaultAsync(x =>
                x.RequestId == requestId && x.UserId == userId && x.BodyHash == bodyHash &&
                x.Path == _httpContext.Request.Path);

            if (idempotencyRecord != null)
            {
                if (idempotencyRecord.Status == HttpRequestEntityStatus.Pending)
                {
                    throw new Exception("This request has been already sent. | Try it with different request-id");
                }

                return JsonSerializer.Deserialize<ServiceResult>(idempotencyRecord.Response);
            }

            await httpRequestTable.AddAsync(new HttpRequestEntity()
            {
                UserId = userId,
                Path = _httpContext.Request.Path,
                BodyHash = bodyHash,
                RequestId = requestId
            });

            await _unitOfWork.SaveAsync();

            var result = await func.Invoke();;

            if (!result.IsSuccess)
            {
                await _unitOfWork.RollbackAsync();

                return result;
            }

            await _unitOfWork.SaveAsync();

            var httpRequestEntity = await httpRequestTable.FirstAsync(x =>
                x.RequestId == requestId && x.UserId == userId && x.BodyHash == bodyHash &&
                x.Path == _httpContext.Request.Path);

            httpRequestEntity.Response = JsonSerializer.Serialize(result);
            httpRequestEntity.Status =
                result.IsSuccess ? HttpRequestEntityStatus.Completed : HttpRequestEntityStatus.Failed;

            await _unitOfWork.SaveAsync();

            await _unitOfWork.CommitAsync();

            return result;
        }
        catch (DbUpdateConcurrencyException)
        {
            await _unitOfWork.RollbackAsync();
    
            return await PersistDbWithIdempotent(func);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();

            throw;
        }
    }

    #endregion
}