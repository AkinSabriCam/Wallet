using System.Security.Cryptography;
using System.Text;
using Domain.Entities;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;

namespace Wallet.Api.Middlewares;

public class IdempotencyMiddleware : IMiddleware
{
    private readonly IServiceProvider _serviceProvider;

    public IdempotencyMiddleware(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // only for methods that modify state (optional)
        if (!HttpMethods.IsPost(context.Request.Method) && !HttpMethods.IsPut(context.Request.Method) &&
            !HttpMethods.IsPatch(context.Request.Method))
        {
            await next(context);
            return;
        }

        StringValues requestId;
        context.Request.Headers.TryGetValue("x-request-id", out requestId);

        if (string.IsNullOrEmpty(requestId))
        {
            await next(context);

            return;
        }

        context.Request.EnableBuffering(); // stream'i başa sarmaya izin verir
        using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        context.Request.Body.Position = 0;

        var hash = HashTheBody(body);
        var userId = context.Request.Headers["x-user-id"];
        var path = context.Request.Path;

        var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<WalletDbContext>();
        var dbTable = dbContext.Set<HttpRequestEntity>();

        if (await dbTable.AsNoTracking()
                .AnyAsync(x => x.BodyHash == hash && x.UserId == userId.ToString() &&
                               x.Path == path.ToString() && x.RequestId == requestId.ToString()))
        {
            context.Response.StatusCode = StatusCodes.Status409Conflict;
            await context.Response.WriteAsync("Duplicate request (idempotent).");

            return;
        }

        await next(context);

        await dbTable.AddAsync(new HttpRequestEntity()
        {
            RequestId = requestId,
            Path = path,
            UserId = userId,
            BodyHash = hash
        });

        await dbContext.SaveChangesAsync();
    }

    private static string HashTheBody(string body)
    {
        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(body);
        var hash = sha.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}