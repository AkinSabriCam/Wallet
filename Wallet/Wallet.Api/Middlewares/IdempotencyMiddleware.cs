using System.Security.Cryptography;
using System.Text;

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
        context.Request.EnableBuffering(); // stream'i başa sarmaya izin verir
        using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        context.Request.Body.Position = 0;

        var hash = HashTheBody(body);

        context.Request.Headers.Append("x-body-hash", hash);
        
        await next(context);
    }

    private static string HashTheBody(string body)
    {
        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(body);
        var hash = sha.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}