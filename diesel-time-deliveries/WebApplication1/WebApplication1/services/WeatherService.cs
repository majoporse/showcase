using System.Text.Json;
using WebApplication1.Models;

namespace WebApplication1.services;

public class WeatherService(IHttpClientFactory factory)
{
    public async Task<WeatherResponse?> GetWeatherData(float lat, float lng)
    {
        var url = $"https://api.open-meteo.com/v1/forecast?latitude={lat}&longitude={lng}&current=temperature_2m,";

        var client = factory.CreateClient();

        var request = await client.GetAsync(url);
        
        request.EnsureSuccessStatusCode();

        var stringRes = await request.Content.ReadAsStringAsync();
        
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        return JsonSerializer.Deserialize<WeatherResponse>(stringRes, options);
    }
}