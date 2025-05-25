using System.Text.Json.Serialization;

namespace WebApplication1.Models;

public class JsonAirportReply
{
    public string Iaco { get; set; }
    public string Iata { get; set; }
    public string Name { get; set; }
    public string Location { get; set; }
    public string Country { get; set; }
    public string CountryCode { get; set; }
    [JsonConverter(typeof(JsonFloatConverter))]
    public float Longitude { get; set; }
    [JsonConverter(typeof(JsonFloatConverter))]
    public float Latitude { get; set; }
    public string Link { get; set; }
    public int Status { get; set; }
}