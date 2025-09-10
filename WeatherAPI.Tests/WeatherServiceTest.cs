using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WeatherAPI.Services;

namespace WeatherAPI.Tests
{
    public class WeatherServiceTest
    {
        [Fact]
        public async Task GetWeatherAsync_Returns_WeatherInfo()
        {
            // Fake response for weather API
            var fakeWeatherJson = @"{
                ""coord"": { ""lat"": 35.7, ""lon"": 51.4 },
                ""main"": { ""temp"": 25.5, ""humidity"": 40 },
                ""wind"": { ""speed"": 3.5 }
            }";

            // Fake response for air pollution API
            var fakeAirJson = @"{
                ""list"": [{
                    ""main"": { ""aqi"": 2 },
                    ""components"": {
                        ""pm2_5"": 12.5,
                        ""pm10"": 25.0,
                        ""co"": 0.4,
                        ""no2"": 15.0,
                        ""so2"": 5.0,
                        ""o3"": 20.0,
                        ""nh3"": 1.5
                    }
                }]
            }";

            var handler = new MockHttpMessageHandler(fakeWeatherJson, fakeAirJson);
            var httpClient = new HttpClient(handler);

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "OpenWeather:ApiKey", "fake-key" }
                })
                .Build();

            var service = new WeatherService(httpClient, config);

            var result = await service.GetWeatherInfoAsync("Tehran");

            Assert.NotNull(result);
            Assert.Equal(25.5, result!.Temperature);
            Assert.Equal(40, result.Humidity);
            Assert.Equal(2, result.AQI);
            Assert.True(result.Pollutants.ContainsKey("pm2_5"));
        }
    }

    public class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly Queue<string> _responses = new();

        public MockHttpMessageHandler(string firstResponse, string secondResponse)
        {
            _responses.Enqueue(firstResponse);
            _responses.Enqueue(secondResponse);
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var responseContent = _responses.Dequeue();
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseContent)
            });
        }
    }
}
