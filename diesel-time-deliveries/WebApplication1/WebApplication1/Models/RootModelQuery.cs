namespace WebApplication1.Models;


/*
- queryAirportTemp - the IATA code of the airport (tree letters) for which to return the temperature 
- queryStockPrice - the identifier of the stock (1-4 letters) for which to get return the price
- queryEval - arithmetic expressions that can contain integer constants, parenthesis, and operations +, -, *, /
 */
public class RootModelQuery
{
    public string? queryAirportTemp { get; set; }
    public string? queryStockPrice  { get; set; }
    public string? queryEval  { get; set; }
    
}