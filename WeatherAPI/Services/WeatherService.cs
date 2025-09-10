using Microsoft.AspNetCore.Http.HttpResults;
using System.Data;
using System.Net;
using System.Text.Json;
using WeatherAPI.Models;

namespace WeatherAPI.Services
{
    public class WeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public WeatherService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _apiKey = config["OpenWeather:ApiKey"] ?? throw new ArgumentNullException("API Key not found");
        }

        public async Task<WeatherInfo?> GetWeatherInfoAsync(string city)

        {


            var weatherUrl = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={_apiKey}&units=metric";


            
                var response = await _httpClient.GetAsync(weatherUrl);
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }
                response.EnsureSuccessStatusCode();

                var weatherResponse = await response.Content.ReadAsStringAsync();


                var weatherJson = JsonDocument.Parse(weatherResponse);

                var coord = weatherJson.RootElement.GetProperty("coord");

                double lat = coord.GetProperty("lat").GetDouble();

                double lon = coord.GetProperty("lon").GetDouble();

                var main = weatherJson.RootElement.GetProperty("main");

                double temp = main.GetProperty("temp").GetDouble();

                int humidity = main.GetProperty("humidity").GetInt32();

                var wind = weatherJson.RootElement.GetProperty("wind");

                double windSpeed = wind.GetProperty("speed").GetDouble();

                var airUrl = $"https://api.openweathermap.org/data/2.5/air_pollution?lat={lat}&lon={lon}&appid={_apiKey}";

                var airResponse = await _httpClient.GetStringAsync(airUrl);

                var airJson = JsonDocument.Parse(airResponse);

                var list = airJson.RootElement.GetProperty("list")[0];
                int aqi = list.GetProperty("main").GetProperty("aqi").GetInt32();
                var components = list.GetProperty("components");
                var pollutants = new Dictionary<string, double>();

                foreach (var component in components.EnumerateObject())
                {
                    pollutants[component.Name] = component.Value.GetDouble();
                }

                return new WeatherInfo
                {
                    Temperature = temp,
                    Humidity = humidity,
                    WindSpeed = windSpeed,
                    AQI = aqi,
                    Latitude = lat,
                    Longitude = lon,
                    Pollutants = pollutants
                };
            

        }
    }
}
