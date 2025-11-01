using Application.DTOs;
using Application.Utilities;

namespace Application.Services;

public interface IAccountService
{
    Task<ServiceResult<AccountDto>> GetAccount(Guid accountId);
    
    Task<ServiceResult<List<AccountDto>>> GetAccounts(Guid userId);

    Task<ServiceResult<AccountDto>> AddAccount(CreateAccountDto dto);
    
    Task<ServiceResult> UpdateAmount(Guid accountId, decimal amount);
}