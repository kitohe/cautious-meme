using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Messaging.API.Services;

namespace Messaging.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IWeatherForecastService _weatherService;
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(IWeatherForecastService weatherForecastService, ILogger<WeatherForecastController> logger)
        {
            _weatherService = weatherForecastService;
            _logger = logger;
        }


        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            List<WeatherForecast> weathers = new();
            
            for (int i = 0; i < 10; i++)
                weathers.Add(_weatherService.GetWeather());

            return weathers;
        }

        [HttpGet("GetRandomString")]
        public string GetRandomString()
        {
            return "SUper random string";
        }
    }
}
