using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WeatherAPI.Services;

namespace WeatherAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherController : ControllerBase
    {
        private readonly WeatherService _weathersrvice;

        private readonly ILogger<WeatherController> _logger;

        public WeatherController(WeatherService weatherService, ILogger<WeatherController> logger)
        {
            _weathersrvice = weatherService;
            _logger = logger;
        }

        [HttpGet("{city}")]
        public async Task<IActionResult> GetWeather(string city)
        {
            try
            {
                var data = await _weathersrvice.GetWeatherInfoAsync(city);
                if (data == null)
                {
                    
                    return NotFound
                        (new { message = $"City '{city}' not found" });


                }


                return Ok(data);
            }
            catch(HttpRequestException ex)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable,
                    new { message = "Weather service unavalible", error = ex.Message });
            }
            catch(TaskCanceledException)
            {
                return StatusCode(StatusCodes.Status504GatewayTimeout,
                    new { message = "Weather service request timed out" });
            }
            catch(Exception ex)
            {
                _logger.LogWarning("Internal server Error!");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Internal server error", error = ex.Message });
            }
        }



    }
}
