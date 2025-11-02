using System.Text;
using System.Text.Json;
using Application.Abstraction;
using Application.Utilities;
using Polly.CircuitBreaker;

namespace Infrastructure.Clients;

public class TransactionClient(HttpClient httpClient) : ITransactionApi
{
    /// <summary>
    /// Decrease
    /// </summary>
    public async Task<ServiceResult> Pay(CreateTransactionDto model)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/transactions/pay-by-wallet");

            request.Content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            request.Headers.Add("x-user-id", model.UserId);
            request.Headers.Add("x-request-id", $"request-id:{new Random().Next(1, 78)}");
        
            var result = await httpClient.SendAsync(request);

            if (result.IsSuccessStatusCode)
            {
                Console.WriteLine($"Decreased wallet amount: {JsonSerializer.Serialize(model)}");

                return JsonSerializer.Deserialize<ServiceResult>(await result.Content.ReadAsStringAsync());
            }

            return ServiceResult.Failed(new List<string>(){"Could not update wallet amount"});
        }
        catch (BrokenCircuitException)
        {
            // devre açık, servis kapalı gibi davran
            // circuit breaker devrede dolayisiyla bir sure istek server'a cikmayacak.
            
            Console.WriteLine("Service temporarily unavailable, please try again later" );
            
            return ServiceResult.Failed(new List<string>(){"Service temporarily unavailable, please try again later"});
        }
    }

    /// <summary>
    /// Increase
    /// </summary>
    public async Task<ServiceResult> CancelPayment(CreateTransactionDto model)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/transactions/cancel-payment");
            request.Content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            request.Headers.Add("x-user-id", model.UserId);

            var result = await httpClient.SendAsync(request);

            if (result.IsSuccessStatusCode)
            {
                Console.WriteLine($"Decreased wallet amount: {JsonSerializer.Serialize(model)}");

                return JsonSerializer.Deserialize<ServiceResult>(await result.Content.ReadAsStringAsync());
            }

            return ServiceResult.Failed(new List<string>(){"Could not update wallet amount"});
        }
        catch (BrokenCircuitException)
        {
            // devre açık, servis kapalı gibi davran
            // circuit breaker devrede dolayisiyla bir sure istek server'a cikmayacak.
            
            Console.WriteLine("Service temporarily unavailable, please try again later" );
            
            return ServiceResult.Failed(new List<string>(){"Service temporarily unavailable, please try again later"});            
        }
    }
}