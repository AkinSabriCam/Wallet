using Api.Middlewares;
using Application.Abstransaction;
using Application.Services;
using Domain.Repositories;
using Infrastructure;
using Infrastructure.Mapping;
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

builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IMapper>(x=> 
    new MyMapper(new Mapper()));

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