using Microsoft.AspNetCore.Mvc;
using Recep.Models;

namespace Recep.Controllers;

[ApiController]
[Route("[controller]")]
public class TestWeatherForecastController : ControllerBase
{
    private readonly static string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<TestWeatherForecastController> _logger;

    public TestWeatherForecastController(ILogger<TestWeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetTestWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        _logger.LogInformation("Return forecast");

        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }
}
