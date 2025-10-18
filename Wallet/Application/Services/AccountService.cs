using Application.Abstransaction;
using Application.DTOs;
using Domain.Entities;
using Domain.Repositories;

namespace Application.Services;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public AccountService(IMapper mapper, IAccountRepository accountRepository, IUnitOfWork unitOfWork)
    {
        _accountRepository= accountRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<AccountDto> GetAccount(Guid accountId)
    {
        var account = await _accountRepository.GetAccountById(accountId);

        if (account == null)
        {
            throw new Exception("Account not found");
        }
        
        return _mapper.Map<AccountDto>(account);
    }

    public async Task<List<AccountDto>> GetAccounts(Guid userId)
    {
        var account = await _accountRepository.GetAccounts(userId);

        return _mapper.Map<List<AccountDto>>(account);    
    }

    public async Task<AccountDto> AddAccount(CreateAccountDto dto)
    {
        if (await _accountRepository.IsExist(dto.UserId, dto.Currency))
        {
            throw new Exception("Account already exists");
        }
        
        var account = await _accountRepository.Add(new AccountEntity()
        {
            Currency = dto.Currency,
            Amount = dto.Amount,
            UserId = dto.UserId,
        });

        await _unitOfWork.SaveAsync();
        
        return _mapper.Map<AccountDto>(account);
    }
}