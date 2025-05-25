using System.Text.Json;
using WebApplication1.Models;

namespace WebApplication1.services;

public class AirportService(IHttpClientFactory _factory)
{
    public async Task<JsonAirportReply?> GetAirportTemperature(string iataCode)
    {
        var url = $"https://airport-data.com/api/ap_info.json?iata={iataCode}";
        var client = _factory.CreateClient();
        
        // Send the HTTP GET request
        var response = await client.GetAsync(url);
        
        // Ensure the response is successful
        response.EnsureSuccessStatusCode();
        var stringres = await response.Content.ReadAsStringAsync();
        
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };
        return JsonSerializer.Deserialize<JsonAirportReply>(stringres, options);
    }
}