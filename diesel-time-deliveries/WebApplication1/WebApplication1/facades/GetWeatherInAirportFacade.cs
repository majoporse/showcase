using WebApplication1.services;

namespace WebApplication1.facades;

public class GetWeatherInAirportFacade(AirportService _airportService, WeatherService _weatherService)
{
    public async Task<double?> GetWeatherInAirport(string iataCode)
    {
        var airportData = await _airportService.GetAirportTemperature(iataCode);
        if (airportData == null)
        {
            return null;
        }

        var weatherData = await _weatherService.GetWeatherData(airportData.Latitude, airportData.Longitude);
        if (weatherData == null)
        {
            return null;
        }

        return weatherData.Current.Temperature2m;
    }
}