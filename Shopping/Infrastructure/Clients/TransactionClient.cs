using System.Text;
using System.Text.Json;
using Application.Abstraction;

namespace Infrastructure.Clients;

public class TransactionClient(HttpClient httpClient) : ITransactionApi
{
    /// <summary>
    /// Decrease
    /// </summary>
    public async Task<bool> Pay(CreateTransactionDto model)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/transactions/pay-by-wallet");
        request.Content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
        request.Headers.Add("x-user-id", model.UserId);
        request.Headers.Add("x-request-id", $"request-id:{new Random().Next(1, 78)}");

        
        var result = await httpClient.SendAsync(request);

        if (result.IsSuccessStatusCode)
        {
            Console.WriteLine($"Decreased wallet amount: {JsonSerializer.Serialize(model)}");

            return true;
        }

        return false;
    }

    /// <summary>
    /// Increase
    /// </summary>
    public async Task<bool> CancelPayment(CreateTransactionDto model)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/transactions/cancel-payment");
        request.Content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
        request.Headers.Add("x-user-id", model.UserId);

        var result = await httpClient.SendAsync(request);

        if (result.IsSuccessStatusCode)
        {
            Console.WriteLine($"Decreased wallet amount: {JsonSerializer.Serialize(model)}");

            return true;
        }

        return false;
    }
}