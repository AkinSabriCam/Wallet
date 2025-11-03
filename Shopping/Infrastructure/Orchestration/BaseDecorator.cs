using Application.Abstraction;
using Application.Utilities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Orchestration;

public class BaseDecorator
{
    private readonly IUnitOfWork _unitOfWork;

    public BaseDecorator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResult> Invoke(Func<Task<ServiceResult>> func)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var result = await func.Invoke();

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
            return await Invoke(func);
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }
    
    public async Task<ServiceResult<TResult>> Invoke<TResult>(Func<Task<ServiceResult<TResult>>> func)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var result = await func.Invoke();

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
            return await Invoke(func);
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }
}


