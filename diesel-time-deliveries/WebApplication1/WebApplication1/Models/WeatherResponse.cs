using System.Text.Json.Serialization;

namespace WebApplication1.Models;

public class WeatherResponse
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double GenerationtimeMs { get; set; }
    public int UtcOffsetSeconds { get; set; }
    public string Timezone { get; set; }
    public string TimezoneAbbreviation { get; set; }
    public double Elevation { get; set; }
    public CurrentUnits CurrentUnits { get; set; }
    public CurrentWeather Current { get; set; }
}

public class CurrentUnits
{
    public string Time { get; set; }
    public string Interval { get; set; }
    public string Temperature2m { get; set; }
    public string ApparentTemperature { get; set; }
}

public class CurrentWeather
{   
    public string Time { get; set; }
    public int Interval { get; set; }
    [JsonPropertyName("temperature_2m")]
    public float Temperature2m { get; set; }
    [JsonPropertyName("apparent_temperature")]
    public double ApparentTemperature { get; set; }
}