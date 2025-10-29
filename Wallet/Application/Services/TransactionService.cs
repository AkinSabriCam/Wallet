using Application.Abstransaction;
using Application.DTOs;
using Domain.Entities;
using Domain.Repositories;

namespace Application.Services;

public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _repository;
    private readonly IAccountRepository _accountRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public TransactionService(ITransactionRepository repository, IMapper mapper,
        IAccountRepository accountRepository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _mapper = mapper;
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<TransactionDto>> GetTransactions(Guid accountId)
    {
        var transactions = await _repository.GetTransactions(accountId);
        
        return _mapper.Map<List<TransactionDto>>(transactions);
    }

    public async Task<TransactionDto> Pay(PaymentDto dto)
    {
        await _unitOfWork.StartTransactionAsync();

        try
        {
            var account = await _accountRepository.GetAccountById(dto.AccountId);

            if (account == null)
            {
                throw new Exception("Account not found");
            }

            if (account.Amount < dto.Amount)
            {
                throw new Exception("Account amount is not enough for this transaction");
            }
        
            var transaction = await _repository.Add(new TransactionEntity()
            {
                Amount = (dto.Amount * -1),
                AccountId = dto.AccountId,
                UserId = dto.UserId,
            });

            account.Amount -= dto.Amount;

            await _unitOfWork.SaveAsync();

            await _unitOfWork.CommitAsync();
            
            return _mapper.Map<TransactionDto>(transaction);
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync();

            throw;
        }
    }
    
    public async Task<TransactionDto> CancelPayment(PaymentDto dto)
    {
        await _unitOfWork.StartTransactionAsync();

        try
        {
            var account = await _accountRepository.GetAccountById(dto.AccountId);

            if (account == null)
            {
                throw new Exception("Account not found");
            }
        
            var transaction = await _repository.Add(new TransactionEntity()
            {
                Amount = dto.Amount,
                AccountId = dto.AccountId,
                UserId = dto.UserId,
            });

            account.Amount += dto.Amount;

            await _unitOfWork.SaveAsync();

            await _unitOfWork.CommitAsync();
            
            return _mapper.Map<TransactionDto>(transaction);
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync();

            throw;
        }
    }
}