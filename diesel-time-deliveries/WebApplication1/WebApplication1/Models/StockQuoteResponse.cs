namespace WebApplication1.Models;

public class StockQuoteResponse
{
    public double C { get; set; } // Current price
    public double D { get; set; } // Change
    public double Dp { get; set; } // Percent change
    public double H { get; set; } // High price of the day
    public double L { get; set; } // Low price of the day
    public double O { get; set; } // Open price of the day
    public double Pc { get; set; } // Previous close price
    public long T { get; set; } // Timestamp
}