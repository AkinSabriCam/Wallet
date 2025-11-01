using Wallet.Api.Middlewares;
using Application.Services;
using Domain.Repositories;
using Infrastructure;
using Infrastructure.Mapping;
using Infrastructure.Orchestration.Account;
using Infrastructure.Orchestration.Transaction;
using Infrastructure.Repositories;
using Infrastructure.UnitOfWork;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using IMapper = Application.Abstransaction.IMapper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<WalletDbContext>(opt => 
    opt.UseNpgsql(builder.Configuration.GetConnectionString("Wallet")));

#region repositories

builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();

#endregion

#region services

builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IAccountService, AccountService>();

#endregion

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IMapper>(x=> new MyMapper(new Mapper()));

builder.Services.AddScoped<ITransactionDecorator, TransactionDecorator>();
builder.Services.AddScoped<IAccountDecorator, AccountDecorator>();

builder.Services.AddScoped<IdempotencyMiddleware>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseMiddleware<IdempotencyMiddleware>();
app.MapControllers();

app.Run();