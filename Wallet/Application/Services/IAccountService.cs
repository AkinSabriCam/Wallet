using Application.DTOs;

namespace Application.Services;

public interface IAccountService
{
    Task<AccountDto> GetAccount(Guid accountId);
    
    Task<List<AccountDto>> GetAccounts(Guid userId);

    Task<AccountDto> AddAccount(CreateAccountDto dto);
}