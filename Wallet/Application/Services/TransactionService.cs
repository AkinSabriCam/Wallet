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


    public async Task<TransactionDto> Add(CreateTransactionDto dto)
    {
        _unitOfWork.
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
            Amount = dto.Amount,
            AccountId = dto.AccountId,
            UserId = dto.UserId,
        });
        
        return _mapper.Map<TransactionDto>(transaction);
    }
}