using System.Text.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using WebApplication1.Models;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace WebApplication1;

public class test
{
    public void Test()
    {
        var stringRes = @"{
  ""latitude"": 48.18,
  ""longitude"": 17.22,
  ""generationtime_ms"": 0.016689300537109375,
  ""utc_offset_seconds"": 0,
  ""timezone"": ""GMT"",
  ""timezone_abbreviation"": ""GMT"",
  ""elevation"": 131.0,
  ""current_units"": {
    ""time"": ""iso8601"",
    ""interval"": ""seconds"",
    ""temperature_2m"": ""°C""
  },
  ""current"": {
    ""time"": ""2025-04-19T14:00"",
    ""interval"": 900,
    ""temperature_2m"": 18.5
  }
}";

        var options = new JsonSerializerOptions
        {
            // PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };
        var aa = JsonSerializer.Deserialize<WeatherResponse>(stringRes, options);

        var json = JsonConvert.DeserializeObject<WeatherResponse>(stringRes, new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy
                {
                    ProcessDictionaryKeys = true, // Ensures dictionary keys are also converted
                    OverrideSpecifiedNames = true // Ensures nested properties are converted
                }
            }
        });
    }
}