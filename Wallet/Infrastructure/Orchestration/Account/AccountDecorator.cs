using Application.DTOs;
using Application.Services;
using Application.Utilities;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Orchestration.Account;

public class AccountDecorator(
    IAccountService accountService,
    IUnitOfWork unitOfWork,
    WalletDbContext walletDbContext,
    IHttpContextAccessor httpContextAccessor)
    : BaseDecorator(unitOfWork, walletDbContext, httpContextAccessor), IAccountDecorator
{
    public Task<ServiceResult<AccountDto>> GetAccount(Guid accountId)
    {
        return accountService.GetAccount(accountId);
    }

    public Task<ServiceResult<List<AccountDto>>> GetAccounts(Guid userId)
    {
        return accountService.GetAccounts(userId);
    }

    public async Task<ServiceResult<AccountDto>> AddAccount(CreateAccountDto dto)
    {
        return await Invoke(()=>accountService.AddAccount(dto), true);
    }

    public async Task<ServiceResult> UpdateAmount(Guid accountId, decimal amount)
    {
        return await Invoke(()=>accountService.UpdateAmount(accountId, amount), true);
    }
}