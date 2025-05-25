using System.Text.Json;
using WebApplication1.Models;

namespace WebApplication1.services;

public class StockService(IHttpClientFactory factory)
{
    public async Task<double> GetStockPrice(string stockSymbol)
    {
        var apiKey = "d01q6m1r01qp8m5uol1gd01q6m1r01qp8m5uol20";
        
        var url = $"https://finnhub.io/api/v1/quote?symbol=AAPL&token={apiKey}";

        var client = factory.CreateClient();

        var request = await client.GetAsync(url);
        
        request.EnsureSuccessStatusCode();

        var stringResponse = await request.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };

        return JsonSerializer.Deserialize<StockQuoteResponse>(stringResponse, options)?.C ?? 0;
    }
}