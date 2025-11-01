using Application.DTOs;
using Application.Services;
using Application.Utilities;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Orchestration.Transaction;

public class TransactionDecorator(
    IUnitOfWork unitOfWork,
    WalletDbContext walletDbContext,
    IHttpContextAccessor httpContextAccessor,
    ITransactionService transactionService)
    : BaseDecorator(unitOfWork, walletDbContext, httpContextAccessor), ITransactionDecorator
{
    public async Task<ServiceResult<List<TransactionDto>>> GetTransactions(Guid accountId)
    {
        return await transactionService.GetTransactions(accountId);
    }

    public async Task<ServiceResult<TransactionDto>> Pay(PaymentDto dto)
    {
        return await Invoke(()=>transactionService.Pay(dto), true);
    }

    public async Task<ServiceResult<TransactionDto>> CancelPayment(PaymentDto dto)
    {
        return await Invoke(()=>transactionService.CancelPayment(dto), true);
    }
}