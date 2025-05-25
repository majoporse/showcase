using Microsoft.AspNetCore.Mvc;
using WebApplication1.facades;
using WebApplication1.Models;
using WebApplication1.services;

namespace WebApplication1.Controllers;

public class RootController(GetWeatherInAirportFacade getWeatherInAirportFacade, StockService stockService) : ControllerBase
{
    [HttpGet("/")]
    public async Task<ActionResult> Get([FromQuery] RootModelQuery query)
    {
        switch (query.queryEval, query.queryAirportTemp, query.queryStockPrice)
        {
            case (null, null, null):
                return BadRequest("No query parameters provided");

            case (var eval, null, null):
                return Ok(EvalService.GetEval(eval));

            case (null, var airportTemp, null):
                return Ok(await getWeatherInAirportFacade.GetWeatherInAirport(airportTemp));

            case (null, null, var stockPrice):
                return Ok(await stockService.GetStockPrice(stockPrice));

            default:
                return BadRequest("Only one query parameter is allowed");
        }
    }
}