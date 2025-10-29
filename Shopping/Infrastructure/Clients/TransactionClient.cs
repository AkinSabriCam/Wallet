using System.Text;
using System.Text.Json;
using Infrastructure.Abstraction;

namespace Infrastructure.Clients;

public class TransactionClient(HttpClient httpClient) : ITransactionApi
{
    /// <summary>
    /// Decrease
    /// </summary>
    public async Task<bool> Pay(DecreaseWalletAmount model)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/transactions/pay-by-wallet");
        request.Content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
        
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
    public async Task<bool> CancelPayment(DecreaseWalletAmount model)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/transactions/cancel-payment");
        request.Content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
        
        var result = await httpClient.SendAsync(request);

        if (result.IsSuccessStatusCode)
        {
            Console.WriteLine($"Decreased wallet amount: {JsonSerializer.Serialize(model)}");

            return true;
        }

        return false;
    }
}