using Application.Abstransaction;
using Application.DTOs;
using Application.Utilities;
using Domain.Entities;
using Domain.Repositories;

namespace Application.Services;

public class TransactionService(
    ITransactionRepository repository,
    IMapper mapper,
    IAccountService accountService,
    IUnitOfWork unitOfWork)
    : ITransactionService
{
    public async Task<ServiceResult<List<TransactionDto>>> GetTransactions(Guid accountId)
    {
        var transactions = await repository.GetTransactions(accountId);
        
        return ServiceResult.Success(mapper.Map<List<TransactionDto>>(transactions));
    }

    public async Task<ServiceResult<TransactionDto>> Pay(PaymentDto dto)
    {
        await unitOfWork.StartTransactionAsync();

        try
        {
            var result = await accountService.UpdateAmount(dto.AccountId, (-1 * Math.Abs(dto.Amount)));

            if (!result.IsSuccess)
            {
                return ServiceResult.Fail<TransactionDto>(result.ErrorMessages);
            }
            
            var transaction = await repository.Add(new TransactionEntity()
            {
                Amount = (dto.Amount * -1),
                AccountId = dto.AccountId,
                UserId = dto.UserId,
            });

            await unitOfWork.SaveAsync();

            await unitOfWork.CommitAsync();
            
            return ServiceResult.Success(mapper.Map<TransactionDto>(transaction));
        }
        catch (Exception)
        {
            await unitOfWork.RollbackAsync();

            throw;
        }
    }
    
    public async Task<ServiceResult<TransactionDto>> CancelPayment(PaymentDto dto)
    {
        await unitOfWork.StartTransactionAsync();

        try
        {
            var updateAmountResult = await accountService.UpdateAmount(dto.AccountId, dto.Amount);

            if (!updateAmountResult.IsSuccess)
            {
                return ServiceResult.Fail<TransactionDto>(updateAmountResult.ErrorMessages);
            }
        
            var transaction = await repository.Add(new TransactionEntity()
            {
                Amount = dto.Amount,
                AccountId = dto.AccountId,
                UserId = dto.UserId,
            });

            await unitOfWork.SaveAsync();

            await unitOfWork.CommitAsync();
            
            return ServiceResult.Success(mapper.Map<TransactionDto>(transaction));
        }
        catch (Exception)
        {
            await unitOfWork.RollbackAsync();

            throw;
        }
    }
}