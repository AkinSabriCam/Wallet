using Application.Abstraction;
using Application.Services;
using Domain.Repositories;
using Infrastructure;
using Infrastructure.Clients;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Http.Resilience;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<ShoppingDbContext>(opt => 
    opt.UseNpgsql(builder.Configuration.GetConnectionString("Shopping")));

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IShoppingBasketRepository, ShoppingBasketRepository>();
builder.Services.AddScoped<IShoppingBasketService, ShoppingBasketService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddHttpClient<ITransactionApi, TransactionClient>(
        client => client.BaseAddress = new Uri("https://localhost:7260"))
    
    // Hepsini tek bir pipeline altında topla
    .AddResilienceHandler("backend-pipeline", pipeline =>
    {
        // 1) İstek başına üst limit
        pipeline.AddTimeout(TimeSpan.FromSeconds(5));
        
        // 2) Retry (sarsmadan, jitter’lı backoff)
        pipeline.AddRetry(new HttpRetryStrategyOptions
        {
            MaxRetryAttempts = 3,
            Delay = TimeSpan.FromMilliseconds(200),
            BackoffType = DelayBackoffType.Constant, // 200, 400, 800 ms
            UseJitter = true,
            ShouldHandle = (args)=>
            {
                return ValueTask.FromResult(args.Outcome switch
                {
                    { Exception: TimeoutRejectedException } => true,
                    { Result: HttpResponseMessage r } when (int)r.StatusCode >= 500 => true,
                    _ => false
                });
            }
        });

        // 3) Circuit Breaker
        pipeline.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
        {
            // 10 saniyelik pencerede en az 20 istek gör,
            // %50 ve üzeri hata varsa devreyi 30 sn aç.
            SamplingDuration = TimeSpan.FromSeconds(10),
            FailureRatio = 0.5,          // %50
            MinimumThroughput = 20,      // pencere başına en az 20 deneme
            BreakDuration = TimeSpan.FromSeconds(30),
            ShouldHandle = (args)=>
            {
                return ValueTask.FromResult(args.Outcome switch
                {
                    { Exception: TimeoutRejectedException } => true,
                    { Result: HttpResponseMessage r } when (int)r.StatusCode >= 500 => true,
                    _ => false
                });
            }
        });
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();